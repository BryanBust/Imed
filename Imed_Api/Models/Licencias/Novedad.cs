namespace Imed_Api.Models.Licencias
{
    public class Novedad
    {
        public string CodigoOperador { get; set; }
        public int RutEmpleador { get; set; }
        public string DvEmpleador { get; set; }
        public string IdUnidadrrhh { get; set; }
        public string Clave { get; set; }

        public int TipoFormulario { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaTemino { get; set; }

    }
}
