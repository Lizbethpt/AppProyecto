using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppProyecto.Models
{
    public class DetalleVenta
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int ID_Venta { get; set; }
        public int ID_Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Importe { get; set; }
    }
}
