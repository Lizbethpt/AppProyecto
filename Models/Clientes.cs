using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppProyecto.Models
{ 
    public class Cliente
    {

    [PrimaryKey]
    public int ID { get; set; }

    public string Nombre { get; set; }

    public string RFC { get; set; }

    public string Domicilio { get; set; }

    public string Ciudad { get; set; }

    public string Estado { get; set; }

    public string Telefono { get; set; }

    public string Correo { get; set; }
    }
}
