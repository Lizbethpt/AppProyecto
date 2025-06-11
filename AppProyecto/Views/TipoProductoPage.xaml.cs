using AppProyecto.Models;
using System.Collections.ObjectModel;

namespace AppProyecto.Views;

public partial class TipoProductoPage : ContentPage
{
    ObservableCollection<TipoProducto> _tipos = new();

    public TipoProductoPage()
    {
        InitializeComponent();
        listaTipos.ItemsSource = _tipos;
        CargarTipos();
    }

    private async void CargarTipos()
    {
        var tipos = await App.Database.GetAllAsync<TipoProducto>();
        _tipos.Clear();
        foreach (var tipo in tipos)
            _tipos.Add(tipo);
    }
    private async void OnMostrarTiposClicked(object sender, EventArgs e)
    {
        var tipos = await App.Database.GetAllAsync<TipoProducto>();
        if (tipos.Count == 0)
        {
            await DisplayAlert("Consulta", "No hay tipos registrados aún.", "OK");
        }
        else
        {
            string mensaje = string.Join("\n", tipos.Select(t => $"ID: {t.ID}, Nombre: {t.Nombre}"));
            await DisplayAlert("Tipos Registrados", mensaje, "OK");
        }
    }
    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (int.TryParse(IdEntry.Text, out int id) && !string.IsNullOrWhiteSpace(NombreEntry.Text))
        {
            var tipoProducto = new TipoProducto
            {
                ID = id,
                Nombre = NombreEntry.Text
            };

            await App.Database.SaveAsync(tipoProducto);
            await DisplayAlert("Éxito", "Tipo de producto guardado", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Ingresa un ID válido y un nombre", "OK");
        }
    }
}
