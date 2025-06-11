using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppProyecto.Models
{
    public class Existencia
    {
        [PrimaryKey]
        public int ID { get; set; }

        public int Unidad_Medida { get; set; }

        public int ID_Producto { get; set; }

        public int Cantidad { get; set; }
    }
}
