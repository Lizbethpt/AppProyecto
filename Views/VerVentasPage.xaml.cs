using AppProyecto.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppProyecto.Views
{
    public partial class VerVentasPage : ContentPage
    {
        ObservableCollection<GrupoVenta> VentasAgrupadas { get; }
            = new ObservableCollection<GrupoVenta>();

        public VerVentasPage()
        {
            InitializeComponent();
            VentasCollectionView.ItemsSource = VentasAgrupadas;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarVentasAsync();
        }

        async Task CargarVentasAsync()
        {
            VentasAgrupadas.Clear();

            var ventas = await App.Database.GetAllAsync<Venta>();
            var productos = await App.Database.GetProductosAsync();

            foreach (var v in ventas)
            {
                var detalles = await App.Database.GetDetallesVentaByVentaId(v.ID);
                var listaDetalles = detalles.Select(d => new DetalleVista
                {
                    ProductoNombre = productos
                        .FirstOrDefault(p => p.ID == d.ID_Producto)?.Nombre
                        ?? "Desconocido",
                    Cantidad = d.Cantidad
                })
                .ToList();
                var grupo = new GrupoVenta
                {
                    Codigo = $"Código: {v.Codigo}",
                    Subtotal = $"Subtotal: ${v.Subtotal:F2}",
                    Total = $"Total: ${v.Total:F2}",
                };
                grupo.Detalles = listaDetalles;

                VentasAgrupadas.Add(grupo);
            }
        }
        public class GrupoVenta : ObservableCollection<DetalleVista>
        {
            public string Codigo { get; set; }
            public string Subtotal { get; set; }
            public string Total { get; set; }

            public List<DetalleVista> Detalles
            {
                set
                {
                    Clear();
                    foreach (var item in value)
                        Add(item);
                }
            }
        }
        public class DetalleVista
        {
            public string ProductoNombre { get; set; }
            public int Cantidad { get; set; }
        }
    }
}
