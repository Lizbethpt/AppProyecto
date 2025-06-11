using AppProyecto.Models;
using CommunityToolkit.Maui.Views;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppProyecto.Views
{
    public partial class ValidarEmbarquePage : ContentPage
    {
        private Venta ventaSeleccionada;
        private List<DetalleVenta> detallesVenta = new();
        private List<Producto> productos = new();
        private Dictionary<int, int> scannedCounts = new();

        public ValidarEmbarquePage()
        {
            InitializeComponent();
        }

        void MostrarPopup(string mensaje, bool esError)
        {
            var popup = new CustomAlertPopup(mensaje, esError);
            this.ShowPopup(popup);
        }

        async void OnEscanearVentaClicked(object sender, EventArgs e)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    MostrarPopup("Debes permitir la cámara.", true);
                    return;
                }
            }

            var reader = new CameraBarcodeReaderView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsDetecting = true,
                CameraLocation = CameraLocation.Rear
            };

            reader.BarcodesDetected += Reader_BarcodesDetected_Venta;

            await Navigation.PushAsync(new ContentPage
            {
                Title = "Escaneando venta...",
                Content = reader
            });
        }

        async void Reader_BarcodesDetected_Venta(object sender, BarcodeDetectionEventArgs e)
        {
            var reader = (CameraBarcodeReaderView)sender;
            reader.IsDetecting = false;

            var codigo = e.Results.FirstOrDefault()?.Value;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Navigation.PopAsync();
                VentaCodigoEntry.Text = codigo;

                ventaSeleccionada = (await App.Database.GetAllAsync<Venta>())
                    .FirstOrDefault(v => v.Codigo == codigo);

                if (ventaSeleccionada == null)
                {
                    MostrarPopup("Venta no encontrada.", true);
                    BackgroundColor = Colors.Red;
                    return;
                }

                if (ventaSeleccionada.Estado == 3)
                {
                    MostrarPopup("Esta venta ya fue entregada.", true);
                    BackgroundColor = Colors.Red;
                    return;
                }

                BackgroundColor = Colors.Yellow;

                detallesVenta = (await App.Database.GetAllAsync<DetalleVenta>())
                    .Where(d => d.ID_Venta == ventaSeleccionada.ID)
                    .ToList();

                productos = await App.Database.GetProductosAsync();

                DetallesListView.ItemsSource = detallesVenta
                    .Select(d => new
                    {
                        ProductoNombre = productos.First(p => p.ID == d.ID_Producto).Nombre,
                        Cantidad = d.Cantidad
                    }).ToList();

                DetallesListView.IsVisible = true;
                EscanearProductoButton.IsEnabled = true;
                EscaneadosListView.IsVisible = true;
                ValidarEmbarqueButton.IsVisible = false;
                scannedCounts.Clear();
            });
        }

        async void OnEscanearProductoClicked(object sender, EventArgs e)
        {
            var reader = new CameraBarcodeReaderView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsDetecting = true,
                CameraLocation = CameraLocation.Rear
            };

            reader.BarcodesDetected += Reader_BarcodesDetected_Producto;

            await Navigation.PushAsync(new ContentPage
            {
                Title = "Escaneando producto...",
                Content = reader
            });
        }

        async void Reader_BarcodesDetected_Producto(object sender, BarcodeDetectionEventArgs e)
        {
            var reader = (CameraBarcodeReaderView)sender;
            reader.IsDetecting = false;

            var codigo = e.Results.FirstOrDefault()?.Value;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Navigation.PopAsync();

                var prod = productos.FirstOrDefault(p => p.Codigo_Barras == codigo);
                if (prod == null)
                {
                    MostrarPopup($"Código '{codigo}' no encontrado.", true);
                    this.BackgroundColor = Colors.Red;
                    await Task.Delay(1000);
                    this.BackgroundColor = Colors.White;
                    return;
                }

                var detalle = detallesVenta.FirstOrDefault(d => d.ID_Producto == prod.ID);
                if (detalle == null)
                {
                    MostrarPopup($"'{prod.Nombre}' no pertenece a la venta.", true);
                    this.BackgroundColor = Colors.Red;
                    await Task.Delay(1000);
                    this.BackgroundColor = Colors.White;
                    return;
                }

                scannedCounts.TryGetValue(prod.ID, out int cnt);
                if (cnt >= detalle.Cantidad)
                {
                    MostrarPopup($"Límite de '{prod.Nombre}' alcanzado.", true);
                    this.BackgroundColor = Colors.Red;
                    await Task.Delay(1000);
                    this.BackgroundColor = Colors.White;
                    return;
                }

                scannedCounts[prod.ID] = cnt + 1;

                var items = scannedCounts.Select(kvp =>
                {
                    var p = productos.First(pr => pr.ID == kvp.Key);
                    var d = detallesVenta.First(dd => dd.ID_Producto == kvp.Key);
                    var c = kvp.Value;
                    Color col = c == d.Cantidad ? Colors.Green
                               : c > d.Cantidad ? Colors.Red
                               : Colors.Black;

                    return new
                    {
                        ProductoNombre = p.Nombre,
                        CantidadEscaneada = $"{c} / {d.Cantidad}",
                        WarningColor = col
                    };
                }).ToList();

                EscaneadosListView.ItemsSource = items;

                bool completa = detallesVenta
                    .All(d => scannedCounts.TryGetValue(d.ID_Producto, out int c2) && c2 >= d.Cantidad);

                if (completa)
                {
                    ValidarEmbarqueButton.IsVisible = true;
                    this.BackgroundColor = Colors.LightGreen;
                }
            });
        }

        async void OnValidarEmbarqueClicked(object sender, EventArgs e)
        {
            ventaSeleccionada.Estado = 3;
            await App.Database.SaveAsync(ventaSeleccionada);
            MostrarPopup("Embarque validado correctamente.", false);
            ValidarEmbarqueButton.IsVisible = false;
            this.BackgroundColor = Colors.Green;
        }
    }
}
