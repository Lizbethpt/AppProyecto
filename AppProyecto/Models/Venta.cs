using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppProyecto.Models
{
    public class Venta
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public int Estado { get; set; } = 0;
        public int ID_Cliente { get; set; }
        public string Codigo { get; set; }
    }
}
