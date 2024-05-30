using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.Text;
using Imed_Api.Models.Licencias;
using Newtonsoft.Json;

namespace Imed_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenciaController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Licencia> GetLicencias()
        {
            return new List<Licencia>
            {
                new Licencia {CodigoOperador="123",
                RutEmpleador = 123456,
                DvEmpleador = "1",
                IdUnidadrrhh = "3",
                Clave = "123456",
                FechaOperacion = DateTime.Now,
                IdLicencia = 1,
                DvLicencia ="1",
                NombreArchivo ="lala",
                TipoArchivo = 1,
                DataArchivo = "1",
                UrlArchivo = "1",
                }
            };
        }

        [Route("Archivo")]
        [HttpPost]
        public async Task<string> GetInfAdjuntoImed([FromBody] Licencia licencia)
        {
            try
            {
                WSImed.WsLMEEmpPortTypeClient licenciaClient = new WSImed.WsLMEEmpPortTypeClient();

                WSImed.LMEInfAdjunto InfoAdjunto = new WSImed.LMEInfAdjunto
                {
                    CodigoOperador = "3",
                    RutEmpleador = licencia.RutEmpleador.ToString(),
                    DvEmpleador = licencia.DvEmpleador,
                    IdUnidadrrhh = "0",
                    Clave = licencia.Clave,
                    FechaOperacion = DateTime.Now,
                    IdLicencia = licencia.IdLicencia.ToString(),
                    DvLicencia = licencia.DvLicencia,
                    NombreArchivo = licencia.NombreArchivo,
                    TipoArchivo = licencia.TipoArchivo.ToString(),
                    DataArchivo = Encoding.UTF8.GetBytes(licencia.DataArchivo),
                    UrlArchivo = licencia.UrlArchivo.ToString()
                };

                WSImed.LMEInfAdjuntoResponse1 response = await licenciaClient.LMEInfAdjuntoAsync(InfoAdjunto);

                string jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comunicarse con el servicio web: " + ex.Message);
                throw;
            }
        }

        [Route("RecuperoZonaA")]
        [HttpPost]
        public async Task<string> GetRecuperoZonaA([FromBody] RecuperoZonaA zonaA)
        {
            try
            {
                WSImed.WsLMEEmpPortTypeClient licenciaClient = new WSImed.WsLMEEmpPortTypeClient();

                WSImed.LMEDetLccZA reCuperoZonaA = new WSImed.LMEDetLccZA
                {
                    CodigoOperador = zonaA.CodigoOperador,
                    RutEmpleador = zonaA.RutEmpleador,
                    DvEmpleador = zonaA.DvEmpleador,
                    IdUnidadrrhh = zonaA.IdUnidadrrhh,
                    Clave = zonaA.Clave,
                    NumLicencia = zonaA.NumLicencia,
                    DigLicencia = zonaA.DigLicencia
                };

                WSImed.LMEDetLccZAResponse1 response = await licenciaClient.LMEDetLccZAAsync(reCuperoZonaA);

                string jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comunicarse con el servicio web: " + ex.Message);
                throw;

            }
        }

        [Route("Novedad")]
        [HttpPost]
        public async Task<string> GetNovedadImed([FromBody] Novedad novedad)
        {
            try
            {
                // Este servicio permite informar las novedades de LME.

                WSImed.WsLMEEmpPortTypeClient licenciaClient = new WSImed.WsLMEEmpPortTypeClient();

                WSImed.LMEEvenLcc novedadEvent = new WSImed.LMEEvenLcc
                {
                    CodigoOperador = novedad.CodigoOperador,
                    RutEmpleador = novedad.RutEmpleador.ToString(),
                    DvEmpleador = novedad.DvEmpleador,
                    IdUnidadrrhh = novedad.IdUnidadrrhh,
                    Clave = novedad.Clave,
                    TipoFormulario = novedad.TipoFormulario.ToString(),
                    FechaDesdeConsulta = novedad.FechaInicio,
                    FechaHastaConsulta = novedad.FechaTemino
                };

                WSImed.LMEEvenLccResponse1 response = await licenciaClient.LMEEvenLccAsync(novedadEvent);

                string jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comunicarse con el servicio web: " + ex.Message);
                throw;
            }
        }

        [Route("InformeZonaC")]
        [HttpPost]
        public async Task<string> GetinformeZonaCimed([FromBody] InformeZonaC licencia)
        {
            try
            {
                // Este servicio permite informar las novedades de LME.

                WSImed.WsLMEEmpPortTypeClient licenciaClient = new WSImed.WsLMEEmpPortTypeClient();

                WSImed.LMEInfSeccC InfoZonaC = new WSImed.LMEInfSeccC
                {
                    CodigoOperador = "3",
                    RutEmpleador = licencia.RutEmpleador.ToString(),
                    DvEmpleador = licencia.DvEmpleador,
                    IdUnidadrrhh = "0",
                    Clave = licencia.Clave,
                    TipoFormulario = "3",
                    IdLicencia = licencia.IdLicencia.ToString(),
                    DvLicencia = licencia.DvLicencia,
                    FecProceso = DateTime.Now,
                    DatosZonaC = Encoding.UTF8.GetBytes(licencia.DataArchivo),
                    MotivoNoRecepcion = licencia.Motivo
                };

                WSImed.LMEInfSeccCResponse1 response = await licenciaClient.LMEInfSeccCAsync(InfoZonaC);

                string jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comunicarse con el servicio web: " + ex.Message);
                throw;
            }
        }

        [Route("Resolucion")]
        [HttpPost]
        public async Task<string> GetResolucionImed([FromBody] ResolucionLicencia licencia)
        {
            try
            {
                // Este servicio permite informar las novedades de LME.

                WSImed.WsLMEEmpPortTypeClient licenciaClient = new WSImed.WsLMEEmpPortTypeClient();

                WSImed.LMEVerPronunciamiento resolucion = new WSImed.LMEVerPronunciamiento
                {
                    CodigoOperador = licencia.CodigoOperador,
                    RutEmpleador = licencia.RutEmpleador.ToString(),
                    DvEmpleador = licencia.DvEmpleador,
                    IdUnidadrrhh = licencia.IdUnidadrrhh,
                    Clave = licencia.Clave,
                    TipoFormulario = licencia.TipoFormulario,
                    IdLicencia = licencia.IdLicencia.ToString(),
                    DvLicencia = licencia.DvLicencia,
                    FechaConsulta = DateTime.Now
                };

                WSImed.LMEVerPronunciamientoResponse1 response = await licenciaClient.LMEVerPronunciamientoAsync(resolucion);

                string jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comunicarse con el servicio web: " + ex.Message);
                throw;
            }
        }

        [Route("Estado")]
        [HttpPost]
        public async Task<string> GetEstadoImed([FromBody] EstadoLicencia licencia)
        {
            try
            {
                WSImed.WsLMEEmpPortTypeClient licenciaClient = new WSImed.WsLMEEmpPortTypeClient();

                WSImed.LMEEstLcc estado = new WSImed.LMEEstLcc
                {
                    CodigoOperador = licencia.CodigoOperador,
                    RutEmpleador = licencia.RutEmpleador.ToString(),
                    DvEmpleador = licencia.DvEmpleador,
                    IdUnidadrrhh = licencia.IdUnidadrrhh,
                    Clave = licencia.Clave,
                    NumLicencia = licencia.IdLicencia.ToString(),
                    DvLicencia = licencia.DvLicencia,
                };

                WSImed.LMEEstLccResponse1 response = await licenciaClient.LMEEstLccAsync(estado);

                string jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comunicarse con el servicio web: " + ex.Message);
                throw;
            }
        }

        [Route("ImedPdf")]
        [HttpPost]
        public async Task<string> GetPdfimed([FromBody] LicenciaPdf licencia)
        {
            try
            {
                // Este servicio permite informar las novedades de LME.

                WSImed.WsLMEEmpPortTypeClient licenciaClient = new WSImed.WsLMEEmpPortTypeClient();

                WSImed.LMEDettLccPdf pdfImed = new WSImed.LMEDettLccPdf
                {
                    CodigoOperador = "3",
                    RutEmpleador = licencia.RutEmpleador.ToString(),
                    DvEmpleador = licencia.DvEmpleador,
                    IdUnidadrrhh = "0",
                    Clave = licencia.Clave,
                    NumLicencia = licencia.NumLicencia.ToString(),
                    DigLicencia = licencia.DigLicencia,
                };

                WSImed.LMEDettLccPdfResponse response = await licenciaClient.LMEDettLccPdfAsync(pdfImed);

                string jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comunicarse con el servicio web: " + ex.Message);
                throw;
            }
        }
    }
}
