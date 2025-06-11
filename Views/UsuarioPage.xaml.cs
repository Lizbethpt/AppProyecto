using AppProyecto.Models;

namespace AppProyecto.Views
{
    public partial class UsuarioPage : ContentPage
    {
        List<TipoUsuario> tiposUsuario;

        public UsuarioPage()
        {
            InitializeComponent();
            CargarTiposUsuario();
        }

        private async void CargarTiposUsuario()
        {
            tiposUsuario = await App.Database.GetTiposUsuarioAsync();
            TipoUsuarioPicker.ItemsSource = tiposUsuario;
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            if (TipoUsuarioPicker.SelectedItem is not TipoUsuario tipoSeleccionado)
            {
                await DisplayAlert("Error", "Selecciona un tipo de usuario", "OK");
                return;
            }

            var usuario = new Usuario
            {
                ID = int.Parse(IdEntry.Text),
                Nombre = NombreEntry.Text,
                TipoUsuarioID = tipoSeleccionado.ID
            };

            await App.Database.SaveUsuarioAsync(usuario);
            await DisplayAlert("Éxito", "Usuario guardado correctamente", "OK");
            LimpiarCampos();
        }

        private async void OnCargarUsuariosClicked(object sender, EventArgs e)
        {
            var usuarios = await App.Database.GetUsuariosAsync();
            usuariosListView.ItemsSource = usuarios;
        }

        private void LimpiarCampos()
        {
            IdEntry.Text = string.Empty;
            NombreEntry.Text = string.Empty;
            TipoUsuarioPicker.SelectedIndex = -1;
        }
    }
}