using AppProyecto.Models;

namespace AppProyecto.Views
{
    public partial class ProductoPage : ContentPage
    {
        public ProductoPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var tipos = await App.Database.GetTiposProductoAsync();
            TipoProductoPicker.ItemsSource = tipos;
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            if (int.TryParse(IdEntry.Text, out int id) &&
                decimal.TryParse(PrecioEntry.Text, out decimal precio) &&
                !string.IsNullOrWhiteSpace(NombreEntry.Text))
            {
                var tipoSeleccionado = TipoProductoPicker.SelectedItem as TipoProducto;

                if (tipoSeleccionado == null)
                {
                    await DisplayAlert("Error", "Debes seleccionar un tipo de producto", "OK");
                    return;
                }

                var producto = new Producto
                {
                    ID = id,
                    Nombre = NombreEntry.Text,
                    Descripcion = DescripcionEntry.Text,
                    Codigo_Barras = CodigoBarrasEntry.Text,
                    Precio = precio,
                    ID_Tipo_Producto = tipoSeleccionado.ID
                };

                await App.Database.SaveProductoAsync(producto);
                await DisplayAlert("Éxito", "Producto guardado", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Revisa los campos obligatorios", "OK");
            }
        }

        private async void OnCargarProductosClicked(object sender, EventArgs e)
        {
            var productos = await App.Database.GetProductosAsync();
            productosListView.ItemsSource = productos;
        }
    }
}
