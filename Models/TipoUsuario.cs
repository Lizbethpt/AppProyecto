using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppProyecto.Models
{
    public class TipoUsuario
    {
        [PrimaryKey]
        public int ID { get; set; }

        public string Nombre { get; set; }
    }
}
