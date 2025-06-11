using AppProyecto.Models;
using SQLite;
namespace AppProyecto.Views
{
    public partial class ClientePage : ContentPage
    {
        public ClientePage()
        {
            InitializeComponent();
        }
        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            var cliente = new Cliente
            {
                ID = int.Parse(IdEntry.Text),
                Nombre = NombreEntry.Text,
                RFC = RfcEntry.Text,
                Domicilio = DomicilioEntry.Text,
                Ciudad = CiudadEntry.Text,
                Estado = EstadoEntry.Text,
                Telefono = TelefonoEntry.Text,
                Correo = CorreoEntry.Text
            };
            await App.Database.SaveClientesAsync(cliente);
            await DisplayAlert("Éxito", "Cliente guardado correctamente", "OK");
            LimpiarCampos();
        }
        private async void OnCargarClientesClicked(object sender, EventArgs e)
        {
            var clientes = await App.Database.GetClientesAsync();
            clientesListView.ItemsSource = clientes;
        }
        private void LimpiarCampos()
        {
            IdEntry.Text = string.Empty;
            NombreEntry.Text = string.Empty;
            RfcEntry.Text = string.Empty;
            DomicilioEntry.Text = string.Empty;
            CiudadEntry.Text = string.Empty;
            EstadoEntry.Text = string.Empty;
            TelefonoEntry.Text = string.Empty;
            CorreoEntry.Text = string.Empty;
        }
    }
}