using AppProyecto.Models;

namespace AppProyecto.Views
{
    public partial class TipoUsuarioPage : ContentPage
    {
        public TipoUsuarioPage()
        {
            InitializeComponent();
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            if (int.TryParse(IdEntry.Text, out int id))
            {
                var tipoUsuario = new TipoUsuario
                {
                    ID = id,
                    Nombre = NombreEntry.Text
                };

                await App.Database.SaveTipoUsuarioAsync(tipoUsuario);
                await DisplayAlert("Éxito", "Tipo de usuario guardado", "OK");
                IdEntry.Text = "";
                NombreEntry.Text = "";
            }
            else
            {
                await DisplayAlert("Error", "ID inválido", "OK");
            }
        }

        private async void OnCargarClicked(object sender, EventArgs e)
        {
            tiposUsuarioListView.ItemsSource = await App.Database.GetTiposUsuarioAsync();
        }
    }
}
