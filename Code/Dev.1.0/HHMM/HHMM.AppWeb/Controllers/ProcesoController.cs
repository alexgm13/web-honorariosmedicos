using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using HHMM.Librerias.EntidadesNegocio;
using HHMM.Librerias.ReglasNegocio;
using General.Librerias.CodigoUsuario;
using General.Librerias.EntidadesNegocio;
using HHMM.AppWeb.Filter;
using System.Text;
using Fleck;
using io = System.IO;
using System.IO.Compression;
using System.Xml;

namespace HHMM.AppWeb.Controllers
{
    [frSeguridad]
    public class ProcesoController : Controller
    {
        string versionJS = ConfigurationManager.AppSettings["VersionJS"];
        public ActionResult PeriodoLista()
        {
            ViewBag.versionJS = versionJS;
            return View();
        }

        public string listarPeriodo(string idSucursal, int anio)
        {
            string rpta = "";
            brPeriodo obrPeriodo = new brPeriodo();
            List<bePeriodo> lbePeriodo = obrPeriodo.listar(idSucursal, anio);
            string listaPeriodo = "";

            if (lbePeriodo != null && lbePeriodo.Count > 0)
            {
                string archivo = Server.MapPath("~//Files//EstPeriodo.txt");
                listaPeriodo = ucCustomSerializer.Serializar(lbePeriodo, '¦', '¯', false, archivo, false, true);
            }

            rpta = String.Format("{0}", listaPeriodo);
            return rpta;
        }

        public string adicionarPeriodo(bePeriodo obePeriodo, string ss)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brPeriodo obrPeriodo = new brPeriodo();
                obePeriodo.UsuarioCreadorId = obeUsuarioLogin.UsuarioId;
                bePeriodoListar obePeriodoListar = obrPeriodo.Adicionar(obePeriodo);
                string listaPerfiles = "";

                if (obePeriodoListar != null)
                {
                    if (obePeriodoListar.ListaPeriodo != null && obePeriodoListar.ListaPeriodo.Count > 0)
                    {
                        string archivo = Server.MapPath("~//Files//EstPeriodo.txt");
                        listaPerfiles = ucCustomSerializer.Serializar(obePeriodoListar.ListaPeriodo, '¦', '¯', false, archivo, false, true);
                    }
                }

                rpta = String.Format("{0}_{1}", listaPerfiles, obePeriodoListar.Rpta);

            }
            return (rpta);
        }

        public string actualizarPeriodo(bePeriodo obePeriodo, string ss)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brPeriodo obrPeriodo = new brPeriodo();
                obePeriodo.UsuarioCreadorId = obeUsuarioLogin.UsuarioId;
                bePeriodoListar obePeriodoListar = obrPeriodo.Actualizar(obePeriodo);
                string listaPeriodos = "";

                if (obePeriodoListar != null)
                {
                    if (obePeriodoListar.ListaPeriodo != null && obePeriodoListar.ListaPeriodo.Count > 0)
                    {
                        string archivo = Server.MapPath("~//Files//EstPeriodo.txt");
                        listaPeriodos = ucCustomSerializer.Serializar(obePeriodoListar.ListaPeriodo, '¦', '¯', false, archivo, false, true);
                    }
                }

                rpta = String.Format("{0}_{1}", listaPeriodos, obePeriodoListar.Rpta);
            }
            return (rpta);
        }

        public string actualizarPeriodoEstado(bePeriodo obePeriodo, string ss)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brPeriodo obrPeriodo = new brPeriodo();
                obePeriodo.UsuarioCreadorId = obeUsuarioLogin.UsuarioId;
                bePeriodoListar obePeriodoListar;
                string tipo = Request.Form["tipo"];
                if (tipo == "0")
                {
                    obePeriodoListar = obrPeriodo.ActualizarEstado(obePeriodo);
                }
                else
                {
                    obePeriodoListar = obrPeriodo.ActualizarEstadocierre(obePeriodo);
                }
                string listaPeriodo = "";
                if (obePeriodoListar != null)
                {
                    if (obePeriodoListar.ListaPeriodo != null && obePeriodoListar.ListaPeriodo.Count > 0)
                    {
                        string archivo = Server.MapPath("~//Files//EstPeriodo.txt");
                        listaPeriodo = ucCustomSerializer.Serializar(obePeriodoListar.ListaPeriodo, '¦', '¯', false, archivo, false, true);
                    }
                }
                rpta = String.Format("{0}¬{1}", listaPeriodo, "Ok");    // obePeriodoListar.Rpta);
            }
            return (rpta);
        }

        public ActionResult ProcesoProvicionLista(string ss, int id)
        {
            AdministracionController Administracion = new AdministracionController();
            beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
            ViewBag.seguridad = (Session["Usuario" + ss] == null ? "" : Administracion.obtenerMenuAccion(id, obeUsuarioLogin.PerfilId).ToString());
            ViewBag.versionJS = versionJS;
            string ip = ConfigurationManager.AppSettings["WebSocketIp"];
            string puerto = ConfigurationManager.AppSettings["WebSocketPuerto"];
            string WebSocketCliente = ConfigurationManager.AppSettings["WebSocketCliente"];

            ViewBag.socket =(String.IsNullOrEmpty(WebSocketCliente)? ip: WebSocketCliente) + ":" + puerto;
            return View();
        }
        [frSeguridad]
        public string listarProceso(beProceso obeProceso, string ss, string or)
        {
            string rpta = "";
            brProceso obrProceso = new brProceso();
            List<beProceso> lbeProceso = obrProceso.listar(obeProceso, or);
            string listaProceso = "";

            if (lbeProceso != null && lbeProceso.Count > 0)
            {
                string archivo = Server.MapPath("~//Files//EstProceso.txt");
                listaProceso = ucCustomSerializer.Serializar(lbeProceso, '¦', '¯', false, archivo);
            }

            rpta = String.Format("{0}", listaProceso);
            return rpta;
        }
        [frSeguridad]
        public string listarProcesoListas(beProceso obeProceso, string ss)
        {
            string rpta = "";
            brProceso obrProceso = new brProceso();
            beProcesoListar obeProcesoListar = obrProceso.listarProcesoListas(obeProceso);
            string listaPeriodo = "";
            string listaTipoAdmision = "";
            string ListaConfiguracionPagos = "";
            if (obeProcesoListar != null && obeProcesoListar.ListaPeriodo.Count > 0)
            {
                string archivo = Server.MapPath("~//Files//EstPeriodo.txt");
                listaPeriodo = ucCustomSerializer.Serializar(obeProcesoListar.ListaPeriodo, '¦', '¯', false, archivo);
            }

            if (obeProcesoListar != null && obeProcesoListar.ListaTipoAdmision.Count > 0)
            {
                string archivo = Server.MapPath("~//Files//EstTipoAdmision.txt");
                listaTipoAdmision = ucCustomSerializer.Serializar(obeProcesoListar.ListaTipoAdmision, '¦', '¯', false, archivo);
            }

            if (obeProcesoListar.ListaConfiguracionPagos != null && obeProcesoListar.ListaConfiguracionPagos.Count > 0)
            {
                ListaConfiguracionPagos = ucCustomSerializer.Serializar(obeProcesoListar.ListaConfiguracionPagos, '¦', '¯', false);
            }

            rpta = String.Format("{0}_{1}_{2}", listaPeriodo, listaTipoAdmision, ListaConfiguracionPagos);
            return rpta;
        }


        [frSeguridad]
        public string adicionarProceso(beProceso obeProceso, string ss, string Checks, string indicadorOA, string indMed, string fecini, string fecfin)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brProceso obrProceso = new brProceso();
                obeProceso.UsuarioCreadorId = obeUsuarioLogin.UsuarioId;
                string[] indicador = Checks.Split('|');
                bool valor1 = (indicador[0] == "true") ? true : false;
                bool valor2 = (indicador[1] == "true") ? true : false;
                bool valor3 = (indicador[2] == "true") ? true : false;
                bool valor4 = (indicador[3] == "true") ? true : false;
                bool valor5 = (indicador[4] == "true") ? true : false;
                bool valor6 = (indicador[5] == "true") ? true : false;
                bool valor7 = (indicador[6] == "true") ? true : false;
                bool valor8 = (indicadorOA == "true") ? true : false;
                bool valor9 = (indMed == "true") ? true : false;
                rpta = obrProceso.Adicionar(obeProceso, valor1, valor2, valor3, valor4, valor5, valor6, valor7, valor8, valor9, DateTime.Parse(fecini), DateTime.Parse(fecfin)).ToString();
            }
            return (rpta);
        }
        [frSeguridad]
        public string actualizarProceso(beProceso obeProceso, string ss, string Checks, string indicadorOA, string indMed, string fecini, string fecfin)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brProceso obrProceso = new brProceso();
                obeProceso.UsuarioCreadorId = obeUsuarioLogin.UsuarioId;
                string[] indicador = Checks.Split('|');
                bool valor1 = (indicador[0] == "true") ? true : false;
                bool valor2 = (indicador[1] == "true") ? true : false;
                bool valor3 = (indicador[2] == "true") ? true : false;
                bool valor4 = (indicador[3] == "true") ? true : false;
                bool valor5 = (indicador[4] == "true") ? true : false;
                bool valor6 = (indicador[5] == "true") ? true : false;
                bool valor7 = (indicador[6] == "true") ? true : false;
                bool valor8 = (indicadorOA == "true") ? true : false;
                bool valor9 = (indMed == "true") ? true : false;
                rpta = obrProceso.Actualizar(obeProceso, valor1, valor2, valor3, valor4, valor5, valor6, valor7, valor8, valor9, DateTime.Parse(fecini), DateTime.Parse(fecfin)).ToString();
            }
            return (rpta);
        }
        [frSeguridad]
        public string actualizarProcesoEstado(beProceso obeProceso, string ss)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brProceso obrProceso = new brProceso();
                obeProceso.UsuarioCreadorId = obeUsuarioLogin.UsuarioId;
                rpta = obrProceso.ActualizarEstado(obeProceso).ToString();
            }
            return (rpta);
        }


        public ActionResult ProvisionLista()
        {
            ViewBag.versionJS = versionJS;
            return View();
        }

        public string ListarProvisionPorId(int id)
        {
            string rpta = "";
            string ListaMedico = "";
            string ListaEspecialidad = "";
            string ListaProvision = "";
            string ListaConfiguracion = "";
            brProvision obrProvision = new brProvision();
            beProvisionDetalleVistaListas obeProvisionDetalleVistaListas = obrProvision.listar(id);

            if (obeProvisionDetalleVistaListas != null && obeProvisionDetalleVistaListas.ListaMedico.Count > 0)
            {
                ListaMedico = ucCustomSerializer.Serializar(obeProvisionDetalleVistaListas.ListaMedico, '¦', '¯', false);
            }
            if (obeProvisionDetalleVistaListas != null && obeProvisionDetalleVistaListas.ListaEspecialidad.Count > 0)
            {
                ListaEspecialidad = ucCustomSerializer.Serializar(obeProvisionDetalleVistaListas.ListaEspecialidad, '¦', '¯', false);
            }
            if (obeProvisionDetalleVistaListas != null && obeProvisionDetalleVistaListas.ListaProvision.Count > 0)
            {
                ListaProvision = ucCustomSerializer.Serializar(obeProvisionDetalleVistaListas.ListaProvision, '¦', '¯', false);
            }
            if (obeProvisionDetalleVistaListas != null && obeProvisionDetalleVistaListas.ListaConfiguracion.Count > 0)
            {
                ListaConfiguracion = ucCustomSerializer.Serializar(obeProvisionDetalleVistaListas.ListaConfiguracion, '¦', '¯', false);
            }
            rpta = String.Format("{0}¬{1}¬{2}¬{3}", ListaMedico, ListaEspecialidad, ListaProvision, ListaConfiguracion);
            return rpta;
        }

        public ActionResult CreacionPlanilla(int esIframe = 0, int id = 0, int seg = 0, string det = "")
        {
            ViewBag.versionJS = versionJS;
            ViewBag.esIframe = esIframe;
            ViewBag.idPlanilla = id;
            ViewBag.idSeg = seg;
            ViewBag.det = det;
            ViewBag.activarperiodo = ConfigurationManager.AppSettings["ComboPlanillaActivacion"];

            return View();
        }

        public string listarProcesoProvisionResumen(string ss, string su)
        {
            string rpta = "";
            brPlanilla obrPlanilla = new brPlanilla();
            List<bePlanillaVistaResumen> lbePlanillaVistaResumen = obrPlanilla.listarPlanillaResumen(su);
            if (lbePlanillaVistaResumen != null && lbePlanillaVistaResumen.Count > 0)
            {
                rpta = ucCustomSerializer.Serializar(lbePlanillaVistaResumen, '¦', '¯', false);
            }
            return (rpta);
        }

        public string listarProcesoProvisionMedicos(string ss)
        {
            string rpta = "";
            int n = (int)Request.InputStream.Length;
            byte[] buffer = new byte[n];
            Request.InputStream.Read(buffer, 0, n);
            string strMedico = Encoding.UTF8.GetString(buffer);
            string[] Medico = strMedico.Split('|');
            brPlanilla obrPlanilla = new brPlanilla();
            List<beProcesoMedico> lbeProcesoMedico = obrPlanilla.listarProcesoMedico(Medico[0], Medico[1], int.Parse(Medico[2]), int.Parse(Medico[3]), DateTime.Parse(Medico[4]), DateTime.Parse(Medico[5]));
            if (lbeProcesoMedico != null && lbeProcesoMedico.Count > 0)
            {
                rpta = ucCustomSerializer.Serializar(lbeProcesoMedico, '¦', '¯', false);
            }
            return (rpta);
        }

        public ActionResult PacienteLista()
        {
            ViewBag.versionJS = versionJS;
            return View();
        }

        public string listarPacientes(string ss)
        {
            string rpta = "";
            int n = (int)Request.InputStream.Length;
            byte[] buffer = new byte[n];
            Request.InputStream.Read(buffer, 0, n);
            string strPaciente = Encoding.UTF8.GetString(buffer);
            string[] Paciente = strPaciente.Split('|');
            brPlanilla obrPlanilla = new brPlanilla();
            List<bePacienteVista> lbePacienteVista = obrPlanilla.listarPacientes(Paciente[0], Paciente[1], Paciente[2]);
            if (lbePacienteVista != null && lbePacienteVista.Count > 0)
            {
                rpta = ucCustomSerializer.Serializar(lbePacienteVista, '¦', '¯', false);
            }
            return (rpta);
        }

        public ActionResult ProvisionDetalleConceptos(string data)
        {
            ViewBag.versionJS = versionJS;
            ViewBag.Datos = data;
            return View();
        }


        public string listarDetalleConceptos(string ss, int pro, int per, int es, string sap = "")
        {
            string rpta = "";
            string ListaDetalle1 = "";
            string ListaDetalle2 = "";
            brPlanilla obrPlanilla = new brPlanilla();
            beProcesoMedicoVista obeProcesoMedicoVista = obrPlanilla.listarDetalledeConceptos(pro, per, es, sap);
            if (obeProcesoMedicoVista != null)
            {
                string datos = obeProcesoMedicoVista.Periodo + "¦" + obeProcesoMedicoVista.NombreCompleto + "¦" + obeProcesoMedicoVista.TipoAdmisionId.ToString() + "¦" + obeProcesoMedicoVista.Especialidad.ToString();
                if (obeProcesoMedicoVista.ListaDetalle != null && obeProcesoMedicoVista.ListaDetalle.Count > 0)
                {
                    ListaDetalle1 = ucCustomSerializer.Serializar(obeProcesoMedicoVista.ListaDetalle, '¦', '¯', false);
                }
                if (obeProcesoMedicoVista.ListaDetalle2 != null && obeProcesoMedicoVista.ListaDetalle2.Count > 0)
                {
                    ListaDetalle2 = ucCustomSerializer.Serializar(obeProcesoMedicoVista.ListaDetalle2, '¦', '¯', false);
                }
                rpta = String.Format("{0}¬{1}¬{2}", datos, ListaDetalle1, ListaDetalle2);
            }
            return rpta;
        }

        public ActionResult ProvisionDetalleConceptosOA(string data)
        {
            ViewBag.versionJS = versionJS;
            ViewBag.Datos = data;
            return View();
        }

        public string listarDetalleConceptosOA(string ss, int pro, string sap = "")
        {
            string rpta = "";
            brPlanilla obrPlanilla = new brPlanilla();
            beDetalleOAListas obeDetalleOAListas = obrPlanilla.listarDetalledeConceptosOA(pro, sap);
            string ListaDetalle1 = "";
            string ListaDetalle2 = "";
            string ListaDetalle3 = "";
            if (obeDetalleOAListas.ListaDetalle1 != null && obeDetalleOAListas.ListaDetalle1.Count > 0)
            {
                ListaDetalle1 = ucCustomSerializer.Serializar(obeDetalleOAListas.ListaDetalle1, '¦', '¯', false, "", false, true);
            }
            if (obeDetalleOAListas.ListaHorario != null && obeDetalleOAListas.ListaHorario.Count > 0)
            {
                string archivo = Server.MapPath("~//Files//EstDetalleOAListasDetalle.txt");
                ListaDetalle2 = ucCustomSerializer.Serializar(obeDetalleOAListas.ListaHorario, '¦', '¯', false, archivo);
            }
            if (obeDetalleOAListas.ListaMontoFijo != null && obeDetalleOAListas.ListaMontoFijo.Count > 0)
            {
                ListaDetalle3 = ucCustomSerializer.Serializar(obeDetalleOAListas.ListaMontoFijo, '¦', '¯', false, "", false, true);
            }
            rpta = String.Format("{0}¬{1}¬{2}", ListaDetalle1, ListaDetalle2, ListaDetalle3);
            return rpta;
        }

        public string CalcularProcesoMedico(string ss, int tt, int cc)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                int n = (int)Request.InputStream.Length;
                byte[] buffer = new byte[n];
                Request.InputStream.Read(buffer, 0, n);
                string strProceso = Encoding.UTF8.GetString(buffer);
                string[] Proceso = strProceso.Split('|');
                string valor1 = Proceso[0];
                int valor2 = int.Parse(Proceso[1]);
                int valor3 = int.Parse(Proceso[2]);
                DateTime valor4 = DateTime.Parse(Proceso[3]);
                DateTime valor5 = DateTime.Parse(Proceso[4]);
                int valor7 = obeUsuarioLogin.UsuarioId;
                bool indicador1 = (Proceso[5] == "true") ? true : false;
                bool indicador2 = (Proceso[6] == "true") ? true : false;
                bool indicador3 = (Proceso[7] == "true") ? true : false;
                bool indicador4 = (Proceso[8] == "true") ? true : false;
                bool indicador5 = (Proceso[9] == "true") ? true : false;
                bool indicador6 = (Proceso[10] == "true") ? true : false;
                bool indicador7 = (Proceso[11] == "true") ? true : false;
                string valor6 = Proceso[12];
                brPlanilla obrPlanilla = new brPlanilla();
                string exito = obrPlanilla.CalculoProcesoMedico(valor1, valor2, valor3, valor4, valor5, valor6, valor7, indicador1, indicador2, indicador3, indicador4, indicador5, indicador6, indicador7);
                //if (exito > -1 && cc == tt)
                //{
                //	rpta = "OK";

                //List<IWebSocketConnection> usuarios = (List<IWebSocketConnection>)HttpContext.Application["usuarios"];
                //string msg = "1";
                //usuarios.ToList().ForEach(s => s.Send(msg));

                //	}
                if (exito != "")
                {
                    rpta = "NEXT|" + exito.ToString();
                }
            }
            return (rpta);
        }


        public string ActualizarProcesoMedico(string ss, string su)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                int n = (int)Request.InputStream.Length;
                byte[] buffer = new byte[n];
                Request.InputStream.Read(buffer, 0, n);
                string strProceso = Encoding.UTF8.GetString(buffer);
                string[] Proceso = strProceso.Split('|');
                string valor1 = Proceso[0];
                int valor2 = obeUsuarioLogin.UsuarioId;
                string valor3 = Proceso[1];
                int valor4 = int.Parse(Proceso[2]);
                brPlanilla obrPlanilla = new brPlanilla();

                if (valor3 != "F")
                {
                    rpta = obrPlanilla.ProcesoMedicoActualizarEstado(valor1, valor2, valor3, valor4, su).ToString();
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    DateTime fechaFin = DateTime.Parse(Proceso[3]);
                    string[] datosConexion = ConfigurationManager.AppSettings["ServidorSPRING-" + obeUsuarioLogin.IdCompania].Split('|');
                    string indicadorServidor = ConfigurationManager.AppSettings["ActivarInterfacesSPRING-" + obeUsuarioLogin.IdCompania];
                    beMedicoAsientoProvisionListas obeMedicoAsientoProvisionListas = null;
                    if (datosConexion.Count() > 1 && datosConexion[0].Trim() != "" && datosConexion[1].Trim() != "" && indicadorServidor == "true")
                    {
                        obeMedicoAsientoProvisionListas = obrPlanilla.generarAsientoProvision(valor4, su, valor2, fechaFin, datosConexion[0], datosConexion[1]);
                    }
                    else
                    {
                        obeMedicoAsientoProvisionListas = obrPlanilla.generarAsientoProvision(valor4, su, valor2, fechaFin, null, null);
                    }
                    if (obeMedicoAsientoProvisionListas != null)
                    {
                        if ((obeMedicoAsientoProvisionListas.listaProduccion != null && obeMedicoAsientoProvisionListas.listaProduccion.Count > 0) || (obeMedicoAsientoProvisionListas.listaMontofijo != null && obeMedicoAsientoProvisionListas.listaMontofijo.Count > 0) || (obeMedicoAsientoProvisionListas.listaHorario != null && obeMedicoAsientoProvisionListas.listaHorario.Count > 0) || (obeMedicoAsientoProvisionListas.listaturno != null && obeMedicoAsientoProvisionListas.listaturno.Count > 0))
                        {
                            if ((obeMedicoAsientoProvisionListas.listaProduccion != null && obeMedicoAsientoProvisionListas.listaProduccion.Count > 0))
                            {
                                sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaProduccion, '¦', '¯', false, ""));
                            }
                            sb.Append("¬");
                            if ((obeMedicoAsientoProvisionListas.listaMontofijo != null && obeMedicoAsientoProvisionListas.listaMontofijo.Count > 0))
                            {
                                sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaMontofijo, '¦', '¯', false, ""));
                            }
                            sb.Append("¬");
                            if ((obeMedicoAsientoProvisionListas.listaHorario != null && obeMedicoAsientoProvisionListas.listaHorario.Count > 0))
                            {
                                sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaHorario, '¦', '¯', false, ""));
                            }
                            sb.Append("¬");
                            if ((obeMedicoAsientoProvisionListas.listaturno != null && obeMedicoAsientoProvisionListas.listaturno.Count > 0))
                            {
                                sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaturno, '¦', '¯', false, ""));
                            }

                            rpta = sb.ToString();

                        }
                        else
                        {

                            rpta = obrPlanilla.ProcesoMedicoActualizarEstado(valor1, valor2, valor3, valor4, su).ToString();
                        }

                    }
                    else
                    {
                        rpta = obrPlanilla.ProcesoMedicoActualizarEstado(valor1, valor2, valor3, valor4, su).ToString();
                    }
                }
            }
            return (rpta);
        }



        public string obtenerListas(string ss, string su, int indoa)
        {
            StringBuilder sb = new StringBuilder();
            brPlanilla obrPlanilla = new brPlanilla();
            bePlanillasListas obePlanillasListas = obrPlanilla.obtenerListas(su, indoa);
            if (obePlanillasListas != null)
            {
                if (obePlanillasListas.ListaTipoAdmision != null && obePlanillasListas.ListaTipoAdmision.Count > 0)
                {
                    sb.Append(ucCustomSerializer.Serializar(obePlanillasListas.ListaTipoAdmision, '¦', '¯', false, ""));
                }
                sb.Append("¬");
                if (obePlanillasListas.ListaPeriodos != null && obePlanillasListas.ListaPeriodos.Count > 0)
                {
                    sb.Append(ucCustomSerializer.Serializar(obePlanillasListas.ListaPeriodos, '¦', '¯', false, ""));
                }
                sb.Append("¬");
                if (obePlanillasListas.ListaPlanillaMedico != null && obePlanillasListas.ListaPlanillaMedico.Count > 0)
                {
                    sb.Append(ucCustomSerializer.Serializar(obePlanillasListas.ListaPlanillaMedico, '¦', '¯', false, ""));
                }
                sb.Append("¬");
                if (obePlanillasListas.ListaAnio != null && obePlanillasListas.ListaAnio.Count > 0)
                {
                    sb.Append(ucCustomSerializer.Serializar(obePlanillasListas.ListaAnio, '¦', '¯', false, ""));
                }
            }
            return (sb.ToString());
        }
        public string obtenerPlanillaDetallelistas(string ss, int indNoOA)
        {
            StringBuilder sb = new StringBuilder();
            int n = (int)Request.InputStream.Length;
            byte[] buffer = new byte[n];
            Request.InputStream.Read(buffer, 0, n);
            string strLista = Encoding.UTF8.GetString(buffer);

            brPlanilla obrPlanilla = new brPlanilla();
            bePlanillaDetalleListas obePlanillaDetalleListas = obrPlanilla.obtenerDetalleListas(strLista, indNoOA);
            if (obePlanillaDetalleListas != null)
            {
                if (obePlanillaDetalleListas.ListaProduccion != null && obePlanillaDetalleListas.ListaProduccion.Count > 0)
                {
                    sb.Append(ucCustomSerializer.Serializar(obePlanillaDetalleListas.ListaProduccion, '¦', '¯', false, "", true, true));
                }
                sb.Append("¬");
                if (obePlanillaDetalleListas.ListaBonificacion != null && obePlanillaDetalleListas.ListaBonificacion.Count > 0)
                {
                    sb.Append(ucCustomSerializer.Serializar(obePlanillaDetalleListas.ListaBonificacion, '¦', '¯', false, "", true, true));
                }
                sb.Append("¬");
                if (obePlanillaDetalleListas.ListaMontoFijo != null && obePlanillaDetalleListas.ListaMontoFijo.Count > 0)
                {
                    sb.Append(ucCustomSerializer.Serializar(obePlanillaDetalleListas.ListaMontoFijo, '¦', '¯', false, "", true, true));
                }
            }
            return sb.ToString();
        }
        public string grabarPlanilla(string ss, string su, int anio)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                int n = (int)Request.InputStream.Length;
                byte[] buffer = new byte[n];
                Request.InputStream.Read(buffer, 0, n);
                string[] strLista = Encoding.UTF8.GetString(buffer).Split('¯');
                brPlanilla obrPlanilla = new brPlanilla();
                rpta = obrPlanilla.grabarPlanilla(strLista[0], strLista[1], anio, su, obeUsuarioLogin.UsuarioId, strLista[2], strLista[3], int.Parse(strLista[4]));
                //if (res > -1)
                //{
                //	rpta = "1";
                //}
            }
            return rpta;
        }

        public string ProvisionPlanillaDescripcion(string ss, string des, int id)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brProcesoPlanilla obrProcesoPlanilla = new brProcesoPlanilla();
                rpta = obrProcesoPlanilla.ActualizarDesProcesoPlanilla(des, id, obeUsuarioLogin.UsuarioId);
            }
            return rpta;
        }

        public ActionResult ProcesoPlanillaLista(string ss, int id)
        {
            if (Session["Usuario" + ss] != null)
            {
                AdministracionController Administracion = new AdministracionController();
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                ViewBag.seguridad = Administracion.obtenerMenuAccion(id, obeUsuarioLogin.PerfilId).ToString();
                ViewBag.versionJS = versionJS;
            }
            return View();
        }

        public string listarProcesoPlanilla(beProcesoPlanilla obeProcesoPlanilla, string ss, string or)
        {
            string rpta = "";
            brProcesoPlanilla obrProcesoPlanilla = new brProcesoPlanilla();
            List<beProcesoPlanilla> lbeProcesoPlanilla = obrProcesoPlanilla.listar(obeProcesoPlanilla, or);
            string listaProceso = "";

            if (lbeProcesoPlanilla != null && lbeProcesoPlanilla.Count > 0)
            {
                string archivo = Server.MapPath("~//Files//EstProcesoPlanilla.txt");
                listaProceso = ucCustomSerializer.Serializar(lbeProcesoPlanilla, '¦', '¯', false, archivo);
            }

            rpta = String.Format("{0}", listaProceso);
            return rpta;
        }

        public string listarProcesoPlanillaPorId(string ss, string su, int pe, string datos, int pro)
        {
            string rpta = "";
            beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
            brProcesoPlanilla obrProcesoPlanilla = new brProcesoPlanilla();
            string[] Lista = datos.Split('|');
            string[] datosConexion = ConfigurationManager.AppSettings["ServidorSPRING-" + obeUsuarioLogin.IdCompania].Split('|');
            string indicadorServidor = ConfigurationManager.AppSettings["ActivarInterfacesSPRING-" + obeUsuarioLogin.IdCompania];
            beProcesoPlanillaVistaListas obeProcesoPlanillaVistaListas = null;
            if (datosConexion.Count() > 1 && datosConexion[0].Trim() != "" && datosConexion[1].Trim() != "" && indicadorServidor == "true")
            {
                obeProcesoPlanillaVistaListas = obrProcesoPlanilla.listarProvisionPlanilla(su, pe, Lista[0], int.Parse(Lista[1]), int.Parse(Lista[2]), pro, datosConexion[0], datosConexion[1]);
            }
            else
            {
                obeProcesoPlanillaVistaListas = obrProcesoPlanilla.listarProvisionPlanilla(su, pe, Lista[0], int.Parse(Lista[1]), int.Parse(Lista[2]), pro, null, null);

            }
            string listaProceso = "";
            string listaTipoDocumento = "";
            if (obeProcesoPlanillaVistaListas != null)
            {
                if (obeProcesoPlanillaVistaListas.ListaProcesoPlanillaVista != null && obeProcesoPlanillaVistaListas.ListaProcesoPlanillaVista.Count > 0)
                {
                    listaProceso = ucCustomSerializer.Serializar(obeProcesoPlanillaVistaListas.ListaProcesoPlanillaVista, '¦', '¯', false);
                }
                if (obeProcesoPlanillaVistaListas.ListaTipoDocumento != null && obeProcesoPlanillaVistaListas.ListaTipoDocumento.Count > 0)
                {
                    listaTipoDocumento = ucCustomSerializer.Serializar(obeProcesoPlanillaVistaListas.ListaTipoDocumento, '¦', '¯', false);
                }
            }
            rpta = String.Format("{0}¬{1}", listaProceso, listaTipoDocumento);
            return rpta;
        }

        public string ActualizarProcesoPlanilla(string ss, int tt, int cc, string su)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                string resultado = "";
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                int n = (int)Request.InputStream.Length;
                byte[] buffer = new byte[n];
                Request.InputStream.Read(buffer, 0, n);
                string strProceso = Encoding.UTF8.GetString(buffer);
                string[] Proceso = strProceso.Split('|');
                string valor1 = Proceso[0];
                string valor2 = Proceso[1];
                int valor5 = obeUsuarioLogin.UsuarioId;
                brProcesoPlanilla obrProcesoPlanilla = new brProcesoPlanilla();
                string[] datosConexion = ConfigurationManager.AppSettings["ServidorSPRING-" + obeUsuarioLogin.IdCompania].Split('|');
                string indicadorServidor = ConfigurationManager.AppSettings["ActivarInterfacesSPRING-" + obeUsuarioLogin.IdCompania];
                if (valor2 != "G")
                {
                    int exito = -1;

                    if (valor2 != "F")
                    {
                        resultado = obrProcesoPlanilla.ProvisionPlanillaActualizarEstado(valor1, valor2, valor5);
                        exito = (resultado == "" ? 0 : -1);
                        if (exito > -1 && cc == tt)
                        {
                            rpta = "OK";
                        }
                        else
                        {
                            rpta = "NEXT|ERROR|" + resultado.ToString();
                        }
                    }
                    else
                    {
                        if (datosConexion.Count() > 1 && datosConexion[0].Trim() != "" && datosConexion[1].Trim() != "" && indicadorServidor == "true")
                        {
                            exito = obrProcesoPlanilla.ProvisionPlanillaObligacionRevertir(int.Parse(Proceso[2]), valor1, su, valor5, datosConexion[0], datosConexion[1]);
                        }
                        else
                        {
                            exito = obrProcesoPlanilla.ProvisionPlanillaObligacionRevertir(int.Parse(Proceso[2]), valor1, su, valor5, null, null);
                        }
                        if (exito > -1)
                        {
                            rpta = "OK";
                        }
                        else
                        {
                            rpta = "NEXT|" + exito.ToString();
                        }
                    }


                }
                else
                {
                    int valor3 = int.Parse(Proceso[2]);
                    if (valor3 != -1)
                    {
                        StringBuilder sb = new StringBuilder();
                        brPlanilla obrPlanilla = new brPlanilla();
                        beMedicoAsientoProvisionListas obeMedicoAsientoProvisionListas = null;
                        if (datosConexion.Count() > 1 && datosConexion[0].Trim() != "" && datosConexion[1].Trim() != "" && indicadorServidor == "true")
                        {
                            obeMedicoAsientoProvisionListas = obrPlanilla.generarObligacionPago(valor3, valor1, su, valor5, datosConexion[0], datosConexion[1]);
                        }
                        else
                        {
                            obeMedicoAsientoProvisionListas = obrPlanilla.generarObligacionPago(valor3, valor1, su, valor5, null, null);
                        }

                        if (obeMedicoAsientoProvisionListas != null)
                        {
                            if ((obeMedicoAsientoProvisionListas.listaProduccion != null && obeMedicoAsientoProvisionListas.listaProduccion.Count > 0) || (obeMedicoAsientoProvisionListas.listaMontofijo != null && obeMedicoAsientoProvisionListas.listaMontofijo.Count > 0) || (obeMedicoAsientoProvisionListas.listaHorario != null && obeMedicoAsientoProvisionListas.listaHorario.Count > 0) || (obeMedicoAsientoProvisionListas.listaturno != null && obeMedicoAsientoProvisionListas.listaturno.Count > 0))
                            {
                                if ((obeMedicoAsientoProvisionListas.listaProduccion != null && obeMedicoAsientoProvisionListas.listaProduccion.Count > 0))
                                {
                                    sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaProduccion, '¦', '¯', false, ""));
                                }
                                sb.Append("¬");
                                if ((obeMedicoAsientoProvisionListas.listaMontofijo != null && obeMedicoAsientoProvisionListas.listaMontofijo.Count > 0))
                                {
                                    sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaMontofijo, '¦', '¯', false, ""));
                                }
                                sb.Append("¬");
                                if ((obeMedicoAsientoProvisionListas.listaHorario != null && obeMedicoAsientoProvisionListas.listaHorario.Count > 0))
                                {
                                    sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaHorario, '¦', '¯', false, ""));
                                }
                                sb.Append("¬");
                                if ((obeMedicoAsientoProvisionListas.listaturno != null && obeMedicoAsientoProvisionListas.listaturno.Count > 0))
                                {
                                    sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaturno, '¦', '¯', false, ""));
                                }

                                rpta = sb.ToString();

                            }
                            else
                            {

                                resultado = obrProcesoPlanilla.ProvisionPlanillaActualizarEstado(valor1, valor2, valor5);
                                int exito = (resultado == "" ? 0 : -1);
                                if (exito > -1 && cc == tt)
                                {
                                    rpta = "OK";
                                }
                                else
                                {
                                    rpta = "NEXT|" + exito.ToString();
                                }
                            }

                        }
                    }
                    else
                    {
                        resultado = obrProcesoPlanilla.ProvisionPlanillaActualizarEstado(valor1, valor2, valor5);
                        int exito = (resultado == "" ? 0 : -1);
                        if (exito > -1 && cc == tt)
                        {
                            rpta = "OK";
                        }
                        else
                        {
                            rpta = "NEXT|" + exito.ToString();
                        }
                    }
                    //else
                    //{
                    //	int exito = obrProcesoPlanilla.ProvisionPlanillaActualizarEstado(valor1, valor2, valor5);
                    //	if (exito > -1 && cc == tt)
                    //	{
                    //		rpta = "OK";
                    //	}
                    //	else
                    //	{
                    //		rpta = "NEXT|" + exito.ToString();
                    //	}
                    //}
                }
            }
            return (rpta);
        }

        public string ActualizarProcesoPlanillaPagado(string ss, int tt, int cc, string su)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                string resultado = "";
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                int n = (int)Request.InputStream.Length;
                byte[] buffer = new byte[n];
                Request.InputStream.Read(buffer, 0, n);
                string strProceso = Encoding.UTF8.GetString(buffer);
                string[] Proceso = strProceso.Split('|');
                string valor1 = Proceso[0];
                string valor2 = Proceso[1];
                int valor5 = obeUsuarioLogin.UsuarioId;
                brProcesoPlanilla obrProcesoPlanilla = new brProcesoPlanilla();
                string[] datosConexion = ConfigurationManager.AppSettings["ServidorSPRING-" + obeUsuarioLogin.IdCompania].Split('|');
                string indicadorServidor = ConfigurationManager.AppSettings["ActivarInterfacesSPRING-" + obeUsuarioLogin.IdCompania];
                if (valor2 != "G")
                {
                    int exito = -1;
                    if (valor2 != "F")
                    {
                        resultado = obrProcesoPlanilla.ProvisionPlanillaActualizarEstado(valor1, valor2, valor5);
                        exito = (resultado == "" ? 0 : -1);
                        if (exito > -1 && cc == tt)
                        {
                            rpta = "OK";
                        }
                        else
                        {
                            rpta = "NEXT|ERROR|" + resultado.ToString();
                        }
                    }
                    else
                    {
                        //if (datosConexion.Count() > 1 && datosConexion[0].Trim() != "" && datosConexion[1].Trim() != "" && indicadorServidor == "true")
                        //{
                        exito = obrProcesoPlanilla.ProvisionPlanillaObligacionRevertirPagado(int.Parse(Proceso[2]), valor1, su, valor5, datosConexion[0], datosConexion[1]);
                        //}
                        //else
                        //{
                        //    exito = obrProcesoPlanilla.ProvisionPlanillaObligacionRevertirPagado(int.Parse(Proceso[2]), valor1, su, valor5, null, null);
                        //}
                        if (exito > -1)
                        {
                            rpta = "OK";
                        }
                        else
                        {
                            rpta = "NEXT|" + exito.ToString();
                        }
                    }
                }
                else
                {
                    int valor3 = int.Parse(Proceso[2]);
                    if (valor3 != -1)
                    {
                        StringBuilder sb = new StringBuilder();
                        brPlanilla obrPlanilla = new brPlanilla();
                        beMedicoAsientoProvisionListas obeMedicoAsientoProvisionListas = null;
                        if (datosConexion.Count() > 1 && datosConexion[0].Trim() != "" && datosConexion[1].Trim() != "" && indicadorServidor == "true")
                        {
                            obeMedicoAsientoProvisionListas = obrPlanilla.generarObligacionPago(valor3, valor1, su, valor5, datosConexion[0], datosConexion[1]);
                        }
                        else
                        {
                            obeMedicoAsientoProvisionListas = obrPlanilla.generarObligacionPago(valor3, valor1, su, valor5, null, null);
                        }

                        if (obeMedicoAsientoProvisionListas != null)
                        {
                            if ((obeMedicoAsientoProvisionListas.listaProduccion != null && obeMedicoAsientoProvisionListas.listaProduccion.Count > 0) || (obeMedicoAsientoProvisionListas.listaMontofijo != null && obeMedicoAsientoProvisionListas.listaMontofijo.Count > 0) || (obeMedicoAsientoProvisionListas.listaHorario != null && obeMedicoAsientoProvisionListas.listaHorario.Count > 0) || (obeMedicoAsientoProvisionListas.listaturno != null && obeMedicoAsientoProvisionListas.listaturno.Count > 0))
                            {
                                if ((obeMedicoAsientoProvisionListas.listaProduccion != null && obeMedicoAsientoProvisionListas.listaProduccion.Count > 0))
                                {
                                    sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaProduccion, '¦', '¯', false, ""));
                                }
                                sb.Append("¬");
                                if ((obeMedicoAsientoProvisionListas.listaMontofijo != null && obeMedicoAsientoProvisionListas.listaMontofijo.Count > 0))
                                {
                                    sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaMontofijo, '¦', '¯', false, ""));
                                }
                                sb.Append("¬");
                                if ((obeMedicoAsientoProvisionListas.listaHorario != null && obeMedicoAsientoProvisionListas.listaHorario.Count > 0))
                                {
                                    sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaHorario, '¦', '¯', false, ""));
                                }
                                sb.Append("¬");
                                if ((obeMedicoAsientoProvisionListas.listaturno != null && obeMedicoAsientoProvisionListas.listaturno.Count > 0))
                                {
                                    sb.Append(ucCustomSerializer.Serializar(obeMedicoAsientoProvisionListas.listaturno, '¦', '¯', false, ""));
                                }

                                rpta = sb.ToString();

                            }
                            else
                            {

                                resultado = obrProcesoPlanilla.ProvisionPlanillaActualizarEstado(valor1, valor2, valor5);
                                int exito = (resultado == "" ? 0 : -1);
                                if (exito > -1 && cc == tt)
                                {
                                    rpta = "OK";
                                }
                                else
                                {
                                    rpta = "NEXT|" + exito.ToString();
                                }
                            }

                        }
                    }
                    else
                    {
                        resultado = obrProcesoPlanilla.ProvisionPlanillaActualizarEstado(valor1, valor2, valor5);
                        int exito = (resultado == "" ? 0 : -1);
                        if (exito > -1 && cc == tt)
                        {
                            rpta = "OK";
                        }
                        else
                        {
                            rpta = "NEXT|" + exito.ToString();
                        }
                    }
                }
            }
            return (rpta);
        }


        public string obtenerAsiento(string ss, string su, int pr)
        {
            string rpta = "";
            brProvision obrProvision = new brProvision();
            List<beAsientoContable> lbeAsientoContable = obrProvision.listarAsientoContable(su, pr);

            if (lbeAsientoContable != null && lbeAsientoContable.Count > 0)
            {
                rpta = ucCustomSerializer.Serializar(lbeAsientoContable, '¦', '¯', false);
            }
            return rpta;
        }

        public string revertirAsientoContable(string ss, string su, int id)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                brProvision obrProvision = new brProvision();
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                string[] datosConexion = ConfigurationManager.AppSettings["ServidorSPRING-" + obeUsuarioLogin.IdCompania].Split('|');
                string indicadorServidor = ConfigurationManager.AppSettings["ActivarInterfacesSPRING-" + obeUsuarioLogin.IdCompania];
                int exito = -1;
                if (datosConexion.Count() > 1 && datosConexion[0].Trim() != "" && datosConexion[1].Trim() != "" && indicadorServidor == "true")
                {
                    exito = obrProvision.RevertirAsientoProvision(su, id, obeUsuarioLogin.UsuarioId, datosConexion[0], datosConexion[1]);
                }
                else
                {
                    exito = obrProvision.RevertirAsientoProvision(su, id, obeUsuarioLogin.UsuarioId, null, null);
                }
                if (exito > -1)
                {
                    rpta = ListarProvisionPorId(id);
                }
                else
                {
                    rpta = "ERROR¬" + exito.ToString();
                }
                //List<beAsientoContable> lbeAsientoContable = obrProvision.listarAsientoContable(su, pr);

                //if (lbeAsientoContable != null && lbeAsientoContable.Count > 0)
                //{
                //	rpta = ucCustomSerializer.Serializar(lbeAsientoContable, '¦', '¯', false);
                //}
            }
            return rpta;
        }

        public ActionResult PlanillaDetalleConceptosOA(string data)
        {
            ViewBag.versionJS = versionJS;
            ViewBag.Datos = data;
            return View();
        }

        public string listarPlanillaDetalleConceptosOA(string ss, int id)
        {
            string rpta = "";
            brProcesoPlanilla obrProcesoPlanilla = new brProcesoPlanilla();
            beDetalleOAListas obeDetalleOAListas = obrProcesoPlanilla.listarDetalledeConceptosOA(id);
            string ListaDetalle1 = "";
            string ListaDetalle2 = "";
            string ListaDetalle3 = "";
            if (obeDetalleOAListas != null)
            {
                if (obeDetalleOAListas.ListaDetalle1 != null && obeDetalleOAListas.ListaDetalle1.Count > 0)
                {
                    string archivo2 = Server.MapPath("~//Files//EstDetalleOAListasDetalle2.txt");
                    ListaDetalle1 = ucCustomSerializer.Serializar(obeDetalleOAListas.ListaDetalle1, '¦', '¯', false, archivo2, false, true);
                }
                if (obeDetalleOAListas.ListaHorario != null && obeDetalleOAListas.ListaHorario.Count > 0)
                {
                    string archivo = Server.MapPath("~//Files//EstDetalleOAListasDetalle.txt");
                    ListaDetalle2 = ucCustomSerializer.Serializar(obeDetalleOAListas.ListaHorario, '¦', '¯', false, archivo);
                }
                if (obeDetalleOAListas.ListaMontoFijo != null && obeDetalleOAListas.ListaMontoFijo.Count > 0)
                {

                    ListaDetalle3 = ucCustomSerializer.Serializar(obeDetalleOAListas.ListaMontoFijo, '¦', '¯', false);
                }
                rpta = String.Format("{0}¬{1}¬{2}", ListaDetalle1, ListaDetalle2, ListaDetalle3);
            }
            return rpta;
        }

        public ActionResult ProcesoDescuentos()
        {
            ViewBag.versionJS = versionJS;
            return View();
        }


        public string ProcesoDescuentoListar()
        {
            string rpta = "";
            string ListaPeriodo = "";
            string ListaProcesoDescuento = "";
            int n = (int)Request.InputStream.Length;
            byte[] buffer = new byte[n];
            Request.InputStream.Read(buffer, 0, n);
            string strProceso = Encoding.UTF8.GetString(buffer);
            string[] Data = strProceso.Split('¦');
            brProcesoDescuento obrProcesoDescuento = new brProcesoDescuento();
            beProcesoDescuentoListas obeProcesoDescuentoListas = obrProcesoDescuento.ProcesoDescuentoListar(int.Parse(Data[0]), Data[1], Data[2], Data[3]);
            if (obeProcesoDescuentoListas != null)
            {
                if (obeProcesoDescuentoListas.ListaPeriodo != null && obeProcesoDescuentoListas.ListaPeriodo.Count > 0)
                {
                    ListaPeriodo = ucCustomSerializer.Serializar(obeProcesoDescuentoListas.ListaPeriodo, '¦', '¯', false);
                }
                if (obeProcesoDescuentoListas.ListaProcesoDescuento != null && obeProcesoDescuentoListas.ListaProcesoDescuento.Count > 0)
                {
                    ListaProcesoDescuento = ucCustomSerializer.Serializar(obeProcesoDescuentoListas.ListaProcesoDescuento, '¦', '¯', false);
                }
                rpta = string.Format("{0}¬{1}", ListaPeriodo, ListaProcesoDescuento);
            }
            return rpta;
        }

        public string leerArchivoExcelPlanilla(string ss, string su, int ti)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                string contenido = "";
                int n = (int)Request.InputStream.Length;
                byte[] buffer = new byte[n];
                Request.InputStream.Read(buffer, 0, n);
                contenido = Encoding.UTF8.GetString(buffer);
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brProcesoPlanilla obrProcesoPlanilla = new brProcesoPlanilla();
                rpta = obrProcesoPlanilla.CargarPlanillaExcel(su, contenido, obeUsuarioLogin.UsuarioId, ti);
            }
            return rpta;
        }

        public string grabarCargaPlanilla(string ss, string su, int anio, int tipo)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                int n = (int)Request.InputStream.Length;
                byte[] buffer = new byte[n];
                Request.InputStream.Read(buffer, 0, n);
                string[] strLista = Encoding.UTF8.GetString(buffer).Split('¯');
                brPlanilla obrPlanilla = new brPlanilla();
                rpta = obrPlanilla.grabarPlanillaCarga(strLista[0], strLista[1], anio, su, obeUsuarioLogin.UsuarioId, tipo);
            }
            return rpta;
        }

        public string AnularProcesoPlanilla(string ss, int id)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brProcesoPlanilla obrProcesoPlanilla = new brProcesoPlanilla();
                string res = obrProcesoPlanilla.AnularProcesoPlanilla(id, obeUsuarioLogin.UsuarioId, obeUsuarioLogin.IdCompania);
                if (res != "")
                {
                    rpta = "1";
                }
            }
            return rpta;
        }

        public string AnularPlanilla(string ss, int id)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brProcesoPlanilla obrProcesoPlanilla = new brProcesoPlanilla();
                string res = obrProcesoPlanilla.AnularPlanilla(id, obeUsuarioLogin.UsuarioId, obeUsuarioLogin.IdCompania);
                if (res != "")
                {
                    rpta = "2";
                }
            }
            return rpta;
        }

        public ActionResult ProcesoAjusteContrato(string AC = "")
        {
            ViewBag.versionJS = versionJS;
            ViewBag.AjusteCambio = AC;
            return View();
        }
        public string ProcesoAjusteContratoListas()
        {
            StringBuilder sb = new StringBuilder();
            int n = (int)Request.InputStream.Length;
            byte[] buffer = new byte[n];
            Request.InputStream.Read(buffer, 0, n);
            string strSucursal = Encoding.UTF8.GetString(buffer);

            brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
            beProcesoAjusteContratoListas obeProcesoAjusteContratoListas = obrProcesoAjusteContrato.ProcesoAjusteContratoListas(strSucursal);

            if (obeProcesoAjusteContratoListas != null)
            {

                if (obeProcesoAjusteContratoListas.listaPeriodo != null && obeProcesoAjusteContratoListas.listaPeriodo.Count > 0)
                {
                    sb.Append(ucCustomSerializer.Serializar(obeProcesoAjusteContratoListas.listaPeriodo, '¦', '¬', false));
                }
                sb.Append("¯");
                if (obeProcesoAjusteContratoListas.listaServicio != null && obeProcesoAjusteContratoListas.listaServicio.Count > 0)
                {
                    sb.Append(ucCustomSerializer.Serializar(obeProcesoAjusteContratoListas.listaServicio, '¦', '¬', false));
                }
            }
            return sb.ToString();
        }
        public string ProcesoAjusteContratoListar()
        {
            string rpta = "";

            int n = (int)Request.InputStream.Length;
            byte[] buffer = new byte[n];
            Request.InputStream.Read(buffer, 0, n);
            string strProceso = Encoding.UTF8.GetString(buffer);
            string[] Data = strProceso.Split('¦');
            brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
            List<beProcesoAjusteContrato> lbeProcesoAjusteContrato = obrProcesoAjusteContrato.ProcesoAjusteContratoListar(Data[0], int.Parse(Data[1]), int.Parse(Data[2]), Data[3], Data[4]);
            if (lbeProcesoAjusteContrato != null && lbeProcesoAjusteContrato.Count > 0)
            {
                rpta = ucCustomSerializer.Serializar(lbeProcesoAjusteContrato, '¦', '¯', false, "", false, true);
            }
            return rpta;
        }

        public string ProcesoAjusteDetalleContratoListar(int opc)
        {
            string rpta = "";

            int n = (int)Request.InputStream.Length;
            byte[] buffer = new byte[n];
            Request.InputStream.Read(buffer, 0, n);
            string strProceso = Encoding.UTF8.GetString(buffer);
            string[] Data = strProceso.Split('¦');
            brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
            if (opc == 1)
            {
                List<beProcesoAjusteContratoDetalle> lbeProcesoAjusteContratoDetalle = obrProcesoAjusteContrato.ProcesoAjusteContratoDetalleListar(Data[0], Data[1], Data[2], int.Parse(Data[3]), int.Parse(Data[4]), Data[5], Data[6], int.Parse(Data[7]), Data[8]);
                if (lbeProcesoAjusteContratoDetalle != null && lbeProcesoAjusteContratoDetalle.Count > 0)
                {
                    rpta = ucCustomSerializer.Serializar(lbeProcesoAjusteContratoDetalle, '¦', '¯', false, "", false, true);
                }
            }
            else
            {
                List<beProcesoAjusteContratoDetalle2> lbeProcesoAjusteContratoDetalle = obrProcesoAjusteContrato.ProcesoAjusteContratoDetalleListar2(Data[0], Data[1], int.Parse(Data[2]), int.Parse(Data[3]), Data[4], Data[5], int.Parse(Data[6]), Data[7], int.Parse(Data[8]));
                if (lbeProcesoAjusteContratoDetalle != null && lbeProcesoAjusteContratoDetalle.Count > 0)
                {
                    rpta = ucCustomSerializer.Serializar(lbeProcesoAjusteContratoDetalle, '¦', '¯', false, "", false, true);
                }
            }
            return rpta;
        }
        public string ProcesoAjusteDetalleContratoListar2(int id = 0)
        {
            string rpta = "";
            brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
            List<beProcesoAjusteContratoDetalle2> lbeProcesoAjusteContratoDetalle = obrProcesoAjusteContrato.ProcesoAjusteContratoListar2(id);
            if (lbeProcesoAjusteContratoDetalle != null && lbeProcesoAjusteContratoDetalle.Count > 0)
            {
                rpta = ucCustomSerializer.Serializar(lbeProcesoAjusteContratoDetalle, '¦', '¯', false, "", false, true);
            }
            return rpta;
        }



        public string ProcesoAjusteDetalleContratoImportarListar(string tipo)
        {
            string rpta = "";
            int n = (int)Request.InputStream.Length;
            byte[] buffer = new byte[n];
            Request.InputStream.Read(buffer, 0, n);
            string strProceso = Encoding.UTF8.GetString(buffer);
            brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
            List<beProcesoAjusteContratoDetalle> lbeProcesoAjusteContratoDetalle = obrProcesoAjusteContrato.ProcesoAjusteContratoDetalleImportarListar(strProceso, tipo);
            if (lbeProcesoAjusteContratoDetalle != null && lbeProcesoAjusteContratoDetalle.Count > 0)
            {
                rpta = ucCustomSerializer.Serializar(lbeProcesoAjusteContratoDetalle, '¦', '¯', false, "", false, true);
            }

            return rpta;
        }

        public string ProcesoAjusteDetalleContratoExcelValidar(string tipo, int id)
        {
            string rpta = "";

            int n = (int)Request.InputStream.Length;
            byte[] buffer = new byte[n];
            Request.InputStream.Read(buffer, 0, n);
            string strProceso = Encoding.UTF8.GetString(buffer);
            brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
            rpta = obrProcesoAjusteContrato.ProcesoAjusteExcelValidar(strProceso, tipo, id);
            return rpta;
        }
        public string ProcesoAjusteDetalleContratoGrabar(string ss, int opc)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                int n = (int)Request.InputStream.Length;
                byte[] buffer = new byte[n];
                Request.InputStream.Read(buffer, 0, n);
                string strProceso = Encoding.UTF8.GetString(buffer);
                string[] data = strProceso.Split('¯');
                int id = -1;
                if (opc == 1)
                {
                    id = -1;
                    brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
                    if (data[0].Equals("0"))
                    {
                        id = obrProcesoAjusteContrato.ProcesoAjusteContratoDetalleAdicionar(data[1], data[2], obeUsuarioLogin.UsuarioId, data[3]);
                    }
                    else
                    {
                        id = obrProcesoAjusteContrato.ProcesoAjusteContratoDetalleActualizar(int.Parse(data[0]), data[1], data[2], obeUsuarioLogin.UsuarioId, data[3]);
                    }
                }
                else
                {
                    id = -1;
                    brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
                    id = obrProcesoAjusteContrato.ProcesoAjusteContratoDetalleAdicionar2(int.Parse(data[0]), obeUsuarioLogin.UsuarioId, data[1]);
                    //if (data[0].Equals("0"))
                    //{
                    //	id = obrProcesoAjusteContrato.ProcesoAjusteContratoDetalleAdicionar2(int.Parse(data[0]),obeUsuarioLogin.UsuarioId, data[1]);
                    //}
                    //else
                    //{
                    //	id = obrProcesoAjusteContrato.ProcesoAjusteContratoDetalleActualizar(int.Parse(data[0]), data[1], data[2], obeUsuarioLogin.UsuarioId, data[3]);
                    //}
                }
                if (id > -1) rpta = id.ToString();
            }

            return rpta;
        }
        public string ProcesoAjusteContratoActualizarEstado(string ss, int id, string est)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                int n = (int)Request.InputStream.Length;
                byte[] buffer = new byte[n];
                Request.InputStream.Read(buffer, 0, n);
                string strProceso = Encoding.UTF8.GetString(buffer);
                string[] data = strProceso.Split('¯');
                int ide = -1;
                brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
                ide = obrProcesoAjusteContrato.ProcesoAjusteContratoActualizarEstado(id, est, obeUsuarioLogin.UsuarioId);

                if (ide > -1) rpta = ide.ToString();
            }

            return rpta;
        }
        public string ProcesoAjusteContratoCalcular(string ss, string su, int id, int tt, int cc)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                int n = (int)Request.InputStream.Length;
                byte[] buffer = new byte[n];
                Request.InputStream.Read(buffer, 0, n);
                string strProceso = Encoding.UTF8.GetString(buffer);
                string[] data = strProceso.Split('¬');


                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];
                brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
                string exito = obrProcesoAjusteContrato.ProcesoAjusteContratoCalcular(id, obeUsuarioLogin.UsuarioId, su, strProceso);
                if (exito != "")
                {
                    rpta = "NEXT¬" + exito + "¬" + data.Count().ToString();
                }
            }

            return rpta;
        }
        public string ProcesoAjusteDetalleContratoPorId(string ss, int id)
        {
            string rpta = "";
            if (Session["Usuario" + ss] != null)
            {
                beUsuarioLogin obeUsuarioLogin = (beUsuarioLogin)Session["Usuario" + ss];

                brProcesoAjusteContrato obrProcesoAjusteContrato = new brProcesoAjusteContrato();
                List<beProcesoAjusteContratoDetalle> lbeProcesoAjusteContratoDetalle = obrProcesoAjusteContrato.ProcesoAjusteContratoDetallePorId(id);
                if (lbeProcesoAjusteContratoDetalle != null && lbeProcesoAjusteContratoDetalle.Count > 0)
                {
                    rpta = ucCustomSerializer.Serializar(lbeProcesoAjusteContratoDetalle, '¦', '¯', false, "", false, true);
                }
            }

            return rpta;
        }
    }
}
