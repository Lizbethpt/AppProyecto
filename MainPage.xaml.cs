namespace AppProyecto.Views
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnTipoProductoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TipoProductoPage());
        }
        private async void OnVBClcked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ValidarEmbarquePage());
        }
        private async void OnProductoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductoPage());
        }
        private async void OnExistenciaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExistenciaPage());
        }
        private async void OnTipoUsuarioClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TipoUsuarioPage());
        }
        private async void OnUsuarioClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UsuarioPage());
        }
        private async void OnClienteClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ClientePage());
        }
        private async void OnRegVentaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new VentasPage());
        }
        private async void OnRegTicketClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistorialVentasPage());
        }
        //ruta db
        private async void OnVClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new VerVentasPage());
        }
        private void AbrirCarpetaBD_Clicked(object sender, EventArgs e)
        {
#if WINDOWS
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "embarques.db3");
            string folderPath = Path.GetDirectoryName(dbPath);
            System.Diagnostics.Process.Start("explorer.exe", folderPath);
#else
                    DisplayAlert("No soportado", "Esta función solo está disponible en Windows.", "OK");
#endif
        }

    }

}