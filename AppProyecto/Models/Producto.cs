using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppProyecto.Models
{
    public class Producto
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Codigo_Barras { get; set; }
        public int ID_Tipo_Producto { get; set; }
        public decimal Precio { get; set; } 
    } 
}
