using AppProyecto.Models;
using Microsoft.Maui.Controls;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using ZXing;
using SkiaSharp;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;

namespace AppProyecto.Views
{
    public partial class VentasPage : ContentPage
    {
        private readonly ObservableCollection<DetalleVenta> _detalles = new();
        public ObservableCollection<ProductoDetalleVisual> DetallesVisuales { get; }
            = new ObservableCollection<ProductoDetalleVisual>();
        private List<DetalleVenta> detallesVentaPDF;
        private string codigoVentaPDF;
        private int clienteIdPDF;
        private List<Producto> productosPDF;
        private List<Producto> productosDisponibles = new();
        public VentasPage()
        {
            InitializeComponent();
            BindingContext = this;
            RegistrarVentaButton.IsEnabled = false;
            GenerarPdfButton.IsEnabled = false;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var existencias = await App.Database.GetExistenciasAsync();
            var productos = await App.Database.GetProductosAsync();
            productosDisponibles = productos
                .Where(p => existencias.Any(e => e.ID_Producto == p.ID && e.Cantidad > 0))
                .ToList();
            ProductoPicker.ItemsSource = productosDisponibles;
        }
        private async void OnAgregarProductoClicked(object sender, EventArgs e)
        {
            if (!(ProductoPicker.SelectedItem is Producto producto)
                || !int.TryParse(CantidadEntry.Text, out int cantidad))
            {
                await DisplayAlert("Error", "Selecciona un producto y cantidad válidos.", "OK");
                return;
            }
            var existencia = (await App.Database.GetExistenciasAsync())
                              .FirstOrDefault(e => e.ID_Producto == producto.ID);
            if (existencia == null || existencia.Cantidad < cantidad)
            {
                await DisplayAlert("Error", "No hay suficiente existencia.", "OK");
                return;
            }
            var detalle = new DetalleVenta
            {
                ID_Producto = producto.ID,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio,
                Importe = producto.Precio * cantidad
            };
            _detalles.Add(detalle);
            DetallesVisuales.Add(new ProductoDetalleVisual
            {
                Nombre = producto.Nombre,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            });
            CantidadEntry.Text = string.Empty;
            ProductoPicker.SelectedItem = null;
            RegistrarVentaButton.IsEnabled = _detalles.Any();
        }
        private async void OnRegistrarVentaClicked(object sender, EventArgs e)
        {
            if (!_detalles.Any())
            {
                await DisplayAlert("Error", "No hay productos en la venta.", "OK");
                return;
            }
            if (!int.TryParse(ClienteEntry.Text, out int idCliente))
            {
                await DisplayAlert("Error", "ID de cliente inválido.", "OK");
                return;
            }
            decimal subtotal = _detalles.Sum(d => d.Importe);
            decimal total = subtotal;
            string codigo = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
            var venta = new Venta
            {
                ID_Cliente = idCliente,
                Codigo = codigo,
                Subtotal = subtotal,
                Total = total,
                Estado = 1
            };
            await App.Database.SaveAsync(venta);
            var ventaGuardada = (await App.Database.GetAllAsync<Venta>())
                                    .OrderByDescending(v => v.ID)
                                    .First();
            foreach (var det in _detalles)
            {
                det.ID_Venta = ventaGuardada.ID;
                await App.Database.SaveAsync(det);

                var exist = (await App.Database.GetExistenciasAsync())
                            .FirstOrDefault(e => e.ID_Producto == det.ID_Producto);
                if (exist != null)
                {
                    exist.Cantidad -= det.Cantidad;
                    await App.Database.SaveExistenciaAsync(exist);
                }
            }

            await DisplayAlert("Éxito", $"Venta registrada con código: {codigo}", "OK");
            codigoVentaPDF = codigo;
            clienteIdPDF = idCliente;
            detallesVentaPDF = _detalles.ToList();
            productosPDF = productosDisponibles.ToList();
            _detalles.Clear();
            DetallesVisuales.Clear();
            RegistrarVentaButton.IsEnabled = false;
            GenerarPdfButton.IsEnabled = true;
            CodigoEntry.Text = codigo;
            ClienteEntry.Text = string.Empty;
        }
        private async void OnGenerarPDFClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(codigoVentaPDF)
                || detallesVentaPDF == null
                || !detallesVentaPDF.Any())
            {
                await DisplayAlert("Error", "No hay datos para generar PDF.", "OK");
                return;
            }

            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 12);
            double y = 40;
            gfx.DrawString("DETALLE DE VENTA",
                new XFont("Arial", 16, XFontStyle.Bold),
                XBrushes.Black,
                new XRect(0, y, page.Width, 40),
                XStringFormats.Center);
            y += 40;
            gfx.DrawString($"Código: {codigoVentaPDF}", font, XBrushes.Black, 40, y); y += 20;
            gfx.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", font, XBrushes.Black, 40, y); y += 20;
            gfx.DrawString($"ID Cliente: {clienteIdPDF}", font, XBrushes.Black, 40, y); y += 30;

            gfx.DrawString("Producto               Cantidad         Precio",
                font, XBrushes.Black, 40, y); y += 20;

            foreach (var d in detallesVentaPDF)
            {
                var prod = productosPDF.FirstOrDefault(p => p.ID == d.ID_Producto);
                if (prod == null) continue;

                string linea = $"{prod.Nombre,-20} {d.Cantidad,-10} ${d.PrecioUnitario}";
                gfx.DrawString(linea, font, XBrushes.Black, 40, y);
                y += 20;
            }

            y += 20;
            gfx.DrawString($"TOTAL: ${detallesVentaPDF.Sum(d => d.Importe)}",
                new XFont("Arial", 12, XFontStyle.Bold),
                XBrushes.Black, 40, y);
            y += 40;
            var writer = new BarcodeWriter<SKBitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions { Width = 300, Height = 100 },
                Renderer = new SKBitmapRenderer()
            };
            var barcode = writer.Write(codigoVentaPDF);
            using var ms = new MemoryStream();
            barcode.Encode(ms, SKEncodedImageFormat.Png, 100);
            ms.Position = 0;
            var img = XImage.FromStream(() => ms);
            gfx.DrawImage(img, 40, y);
            using var pdfStream = new MemoryStream();
            document.Save(pdfStream);
            pdfStream.Position = 0;
            var path = Path.Combine(FileSystem.CacheDirectory, $"Venta_{codigoVentaPDF}.pdf");
            using var fs = File.Create(path);
            pdfStream.CopyTo(fs);

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(path)
            });
        }
    }
    public class ProductoDetalleVisual
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}