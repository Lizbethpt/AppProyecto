using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppProyecto.Models
{
    public class Usuario
    {
        [PrimaryKey]
        public int ID { get; set; }

        public string Nombre { get; set; }

        public int TipoUsuarioID { get; set; }
    }
}
