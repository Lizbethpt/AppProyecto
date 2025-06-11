using AppProyecto.Models;
using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace AppProyecto.Views
{
    public partial class ExistenciaPage : ContentPage
    {
        private List<Producto> productosList;

        public ExistenciaPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            productosList = await App.Database.GetProductosAsync();
            ProductoPicker.ItemsSource = productosList;
        }

        private void OnProductoSeleccionado(object sender, EventArgs e)
        {
           
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            if (ProductoPicker.SelectedItem is Producto productoSeleccionado)
            {
                var existencia = new Existencia
                {
                    ID = int.Parse(HistorialIdEntry.Text),
                    Unidad_Medida = int.Parse(UnidadMedidaEntry.Text),
                    ID_Producto = productoSeleccionado.ID,
                    Cantidad = int.Parse(CantidadEntry.Text)
                };

                await App.Database.SaveExistenciaAsync(existencia);
                await DisplayAlert("Éxito", "Existencia registrada correctamente", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Selecciona un producto válido", "OK");
            }
        }

        private async void OnCargarExistenciasClicked(object sender, EventArgs e)
        {
            var existencias = await App.Database.GetExistenciasAsync();
            existenciasListView.ItemsSource = existencias;
        }
    }
}
