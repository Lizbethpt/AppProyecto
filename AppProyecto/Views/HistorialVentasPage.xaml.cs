using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppProyecto.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using SkiaSharp;
using ZXing;
namespace AppProyecto.Views
{
    public partial class HistorialVentasPage : ContentPage
    {
        private List<Venta> ventas = new();
        private Venta ventaSeleccionada;
        private List<DetalleVenta> detalles = new();
        private List<Producto> productos = new();

        public HistorialVentasPage()
        {
            InitializeComponent();

            if (GlobalFontSettings.FontResolver == null)
                GlobalFontSettings.FontResolver = new EmbeddedFontResolver();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ventas = await App.Database.GetAllAsync<Venta>();
            productos = await App.Database.GetProductosAsync();
            VentasPicker.ItemsSource = ventas;
        }

        private async void OnVentaSeleccionada(object sender, EventArgs e)
        {
            ventaSeleccionada = VentasPicker.SelectedItem as Venta;
            if (ventaSeleccionada != null)
            {
                detalles = (await App.Database.GetAllAsync<DetalleVenta>())
                    .Where(d => d.ID_Venta == ventaSeleccionada.ID)
                    .ToList();

                DetallesListView.ItemsSource = detalles.Select(d =>
                {
                    var producto = productos.FirstOrDefault(p => p.ID == d.ID_Producto);
                    return new
                    {
                        Producto = producto?.Nombre ?? "<desconocido>",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    };
                }).ToList();
            }
        }

        private async void OnGenerarPDFClicked(object sender, EventArgs e)
        {
            if (ventaSeleccionada == null || !detalles.Any())
            {
                await DisplayAlert("Error", "Selecciona una venta válida", "OK");
                return;
            }

            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var regularFont = new XFont("Arial", 12);
            var boldFont = new XFont("Arial", 16, XFontStyle.Bold);
            double y = 40;

            gfx.DrawString("DETALLE DE VENTA", boldFont, XBrushes.Black, new XRect(0, y, page.Width, 40), XStringFormats.Center);
            y += 40;

            gfx.DrawString($"Código: {ventaSeleccionada.Codigo}", regularFont, XBrushes.Black, 40, y); y += 20;
            gfx.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", regularFont, XBrushes.Black, 40, y); y += 20;
            gfx.DrawString($"ID Cliente: {ventaSeleccionada.ID_Cliente}", regularFont, XBrushes.Black, 40, y); y += 30;
            gfx.DrawString("Producto        Cantidad        Precio", regularFont, XBrushes.Black, 40, y); y += 20;

            foreach (var d in detalles)
            {
                var producto = productos.FirstOrDefault(p => p.ID == d.ID_Producto);
                if (producto != null)
                {
                    string linea = $"{producto.Nombre,-15} {d.Cantidad,-10} ${d.PrecioUnitario}";
                    gfx.DrawString(linea, regularFont, XBrushes.Black, 40, y);
                    y += 20;
                }
            }

            y += 20;
            gfx.DrawString($"TOTAL: ${ventaSeleccionada.Total}", boldFont, XBrushes.Black, 40, y);
            y += 40;

            var writer = new BarcodeWriter<SKBitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 100
                },
                Renderer = new SKBitmapRenderer()
            };

            var barcodeBitmap = writer.Write(ventaSeleccionada.Codigo);
            using var imageStream = new MemoryStream();
            barcodeBitmap.Encode(imageStream, SKEncodedImageFormat.Png, 100);
            imageStream.Position = 0;
            var barcodeImage = XImage.FromStream(() => imageStream);
            gfx.DrawImage(barcodeImage, 40, y);

            using var pdfStream = new MemoryStream();
            document.Save(pdfStream);
            pdfStream.Position = 0;

            var fileName = $"Venta_{ventaSeleccionada.Codigo}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            using (var fileStream = File.Create(filePath))
                pdfStream.CopyTo(fileStream);

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
    }
}
