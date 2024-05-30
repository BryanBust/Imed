using Imed_Api.Models.Licencias;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using static ImedApi.Controllers.TestController;

namespace ImedApi.Controllers
{
    [Route("api/test/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
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
                Imed.LicenciasMedicas.WsLMEEmpPortTypeClient licenciaClient = new Imed.LicenciasMedicas.WsLMEEmpPortTypeClient();

                Imed.LicenciasMedicas.LMEInfAdjunto InfoAdjunto = new Imed.LicenciasMedicas.LMEInfAdjunto
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

                Imed.LicenciasMedicas.LMEInfAdjuntoResponse1 response = await licenciaClient.LMEInfAdjuntoAsync(InfoAdjunto);

                string jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comunicarse con el servicio web: " + ex.Message);
                throw;
            }
        }

        //[Route("RecuperoZonaA")]
        [HttpPost]
        public async Task<GenericResponse> GetRecuperoZonaA([FromBody] RecuperoZonaA zonaA)
        {
            try
            {
                RestClientOptions restClientOptions = new RestClientOptions
                {
                    BaseUrl = new Uri("https://wspre.licencia.cl/WsempsWSI.php"),
                };

                RestClient restClient = new RestClient(restClientOptions);

                RestRequest request = new RestRequest();

                request.AddHeader("Content-Type", "text/xml");
                request.AddHeader("Cookie", "SERVERID=im-lmepre-ws01-ng1");

                string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"" +
                                                  "xmlns:urn=\"urn:WsLMEEmp\"" +
                                                  "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
                                                  "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                                                    "<soapenv:Header/>" +
                                                        "<soapenv:Body>" +
                                                            "<urn:LMEDetLccZA soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
                                                            $"<CodigoOperador>{zonaA.CodigoOperador}</CodigoOperador>" +
                                                            $"<RutEmpleador>{zonaA.RutEmpleador}</RutEmpleador>" +
                                                            $"<DvEmpleador>{zonaA.DvEmpleador}</DvEmpleador>" +
                                                            $"<IdUnidadrrhh>{zonaA.IdUnidadrrhh}</IdUnidadrrhh>" +
                                                            $"<Clave>{zonaA.Clave}</Clave>" +
                                                            $"<NumLicencia>{zonaA.NumLicencia}</NumLicencia> " +
                                                            $"<DigLicencia>{zonaA.DigLicencia}</DigLicencia>" +
                                                        "</urn:LMEDetLccZA>" +
                                                    "</soapenv:Body>" +
                                                  "</soapenv:Envelope>";

                request.AddParameter("text/xml", xml, ParameterType.RequestBody);

                RestResponse restResponse = restClient.ExecutePost(request);

                switch (restResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        {
                            XmlDocument xmlDocument = new XmlDocument();

                            string? content = restResponse.Content;

                            if (string.IsNullOrEmpty(content))
                            {
                                throw new Exception("La respuesta del servicio es vacía");
                            }

                            xmlDocument.LoadXml(content);

                            string state = xmlDocument.DocumentElement.GetElementsByTagName("Estado").Item(0).InnerText;

                            string gloss = xmlDocument.DocumentElement.GetElementsByTagName("GloEstado").Item(0).InnerText;

                            string base64 = xmlDocument.DocumentElement.GetElementsByTagName("DctoLme").Item(0).InnerText;

                            ResponseAZone responseAZone = new ResponseAZone(state, gloss, base64);

                            return new GenericResponse
                            {
                                Success = true,
                                StatusCode = (int)System.Net.HttpStatusCode.OK,
                                Message = "Éxito",
                                ResponseAZone = responseAZone
                            };
                        }
                    case System.Net.HttpStatusCode.Unauthorized:
                        {
                            return new GenericResponse
                            {
                                Success = false,
                                StatusCode = (int)System.Net.HttpStatusCode.Unauthorized,
                                Message = "Sin permisos",
                                ResponseAZone = null
                            };
                        }
                    case System.Net.HttpStatusCode.BadRequest:
                        {
                            return new GenericResponse
                            {
                                Success = false,
                                StatusCode = (int)System.Net.HttpStatusCode.BadRequest,
                                Message = "Llamada mal configurada",
                                ResponseAZone = null
                            };
                        }
                    case System.Net.HttpStatusCode.InternalServerError:
                        {
                            return new GenericResponse
                            {
                                Success = false,
                                StatusCode = (int)System.Net.HttpStatusCode.BadRequest,
                                Message = "Error interno de servidor en proveedor",
                                ResponseAZone = null
                            };
                        }
                }

                return new GenericResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener Zona A: " + ex.Message);

                return new GenericResponse
                {
                    Success = false,
                    StatusCode = (int)System.Net.HttpStatusCode.InternalServerError,
                    Message = "Error interno local de servidor",
                    ResponseAZone = null
                };
            }
        }

        [Route("Novedad")]
        [HttpPost]
        public async Task<string> GetNovedadImed([FromBody] Novedad novedad)
        {
            try
            {
                // Este servicio permite informar las novedades de LME.

                Imed.LicenciasMedicas.WsLMEEmpPortTypeClient licenciaClient = new Imed.LicenciasMedicas.WsLMEEmpPortTypeClient();

                Imed.LicenciasMedicas.LMEEvenLcc novedadEvent = new Imed.LicenciasMedicas.LMEEvenLcc
                {
                    CodigoOperador = novedad.CodigoOperador,
                    RutEmpleador = novedad.RutEmpleador,
                    DvEmpleador = novedad.DvEmpleador,
                    IdUnidadrrhh = novedad.IdUnidadrrhh,
                    Clave = novedad.Clave,
                    TipoFormulario = novedad.TipoFormulario.ToString(),
                    FechaDesdeConsulta = novedad.FechaInicio,
                    FechaHastaConsulta = novedad.FechaTemino
                };

                Imed.LicenciasMedicas.LMEEvenLccResponse1 response = await licenciaClient.LMEEvenLccAsync(novedadEvent);

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

                Imed.LicenciasMedicas.WsLMEEmpPortTypeClient licenciaClient = new Imed.LicenciasMedicas.WsLMEEmpPortTypeClient();

                Imed.LicenciasMedicas.LMEInfSeccC InfoZonaC = new Imed.LicenciasMedicas.LMEInfSeccC
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

                Imed.LicenciasMedicas.LMEInfSeccCResponse1 response = await licenciaClient.LMEInfSeccCAsync(InfoZonaC);

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

                Imed.LicenciasMedicas.WsLMEEmpPortTypeClient licenciaClient = new Imed.LicenciasMedicas.WsLMEEmpPortTypeClient();

                Imed.LicenciasMedicas.LMEVerPronunciamiento resolucion = new Imed.LicenciasMedicas.LMEVerPronunciamiento
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

                Imed.LicenciasMedicas.LMEVerPronunciamientoResponse1 response = await licenciaClient.LMEVerPronunciamientoAsync(resolucion);

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
                Imed.LicenciasMedicas.WsLMEEmpPortTypeClient licenciaClient = new Imed.LicenciasMedicas.WsLMEEmpPortTypeClient();

                Imed.LicenciasMedicas.LMEEstLcc estado = new Imed.LicenciasMedicas.LMEEstLcc
                {
                    CodigoOperador = licencia.CodigoOperador,
                    RutEmpleador = licencia.RutEmpleador.ToString(),
                    DvEmpleador = licencia.DvEmpleador,
                    IdUnidadrrhh = licencia.IdUnidadrrhh,
                    Clave = licencia.Clave,
                    NumLicencia = licencia.IdLicencia.ToString(),
                    DvLicencia = licencia.DvLicencia,
                };

                Imed.LicenciasMedicas.LMEEstLccResponse1 response = await licenciaClient.LMEEstLccAsync(estado);

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

                Imed.LicenciasMedicas.WsLMEEmpPortTypeClient licenciaClient = new Imed.LicenciasMedicas.WsLMEEmpPortTypeClient();

                Imed.LicenciasMedicas.LMEDettLccPdf pdfImed = new Imed.LicenciasMedicas.LMEDettLccPdf
                {
                    CodigoOperador = "3",
                    RutEmpleador = licencia.RutEmpleador.ToString(),
                    DvEmpleador = licencia.DvEmpleador,
                    IdUnidadrrhh = "0",
                    Clave = licencia.Clave,
                    NumLicencia = licencia.NumLicencia.ToString(),
                    DigLicencia = licencia.DigLicencia,
                };

                Imed.LicenciasMedicas.LMEDettLccPdfResponse response = await licenciaClient.LMEDettLccPdfAsync(pdfImed);

                string jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comunicarse con el servicio web: " + ex.Message);
                throw;
            }
        }


        public struct GenericResponse
        {
            public bool Success
            {
                get;
                set;
            } = false;

            public int StatusCode
            {
                get;
                set;
            } = 401;

            public string Message
            {
                get;
                set;
            } = string.Empty;

            public ResponseAZone? ResponseAZone
            {
                get;
                set;
            } = null;

            public GenericResponse()
            {

            }
        }

        public struct ResponseAZone
        {
            public string Estado
            {
                get;
                set;
            }

            public string GloEstado
            {
                get;
                set;
            }

            public string DctoLme
            {
                get;
                set;
            }

            [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public XDocument DctoLmeXml
            {
                get;
                set;
            }

            public ResponseAZone(string estado, string gloEstado, string dctoLme)
            {
                this.Estado = estado;

                this.GloEstado = gloEstado;

                this.DctoLme = dctoLme;

                string element = Encoding.UTF8.GetString(Convert.FromBase64String(dctoLme));

                XDocument document = XDocument.Parse(element);

                this.DctoLmeXml = document;
            }
        }
    }
}