namespace Imed_Api.Models.Licencias
{
    public class InformeZonaC
    {
        public string CodigoOperador { get; set; }
        public int RutEmpleador { get; set; }
        public string DvEmpleador { get; set; }
        public string IdUnidadrrhh { get; set; }
        public string Clave { get; set; }
        public int TipoFormulario { get; set; }

        public int IdLicencia { get; set; }
        public string DvLicencia { get; set; }
        public string DataArchivo { get; set; }
        public string Motivo { get; set; }
    }
}
