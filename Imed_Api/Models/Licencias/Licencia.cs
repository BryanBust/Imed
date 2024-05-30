using System.ComponentModel.DataAnnotations;

namespace Imed_Api.Models.Licencias
{
    public class Licencia
    {
        public string CodigoOperador { get; set; }
        public int RutEmpleador { get; set; }
        public string DvEmpleador { get; set; }
        public string IdUnidadrrhh { get; set; }
        public string Clave { get; set; }
        public DateTime FechaOperacion { get; set; }

        public int IdLicencia { get; set; }
        public string DvLicencia { get; set; }
        public string NombreArchivo { get; set; }
        public int TipoArchivo { get; set; }
        public string DataArchivo { get; set; }
        public string UrlArchivo { get; set; }

    }
}
