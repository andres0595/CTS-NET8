using CTS_NET8.Configurations;
using CTS_NET8.Connection;
using CTS_NET8.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.SqlClient;
using System.Net;
using Dapper;
using Newtonsoft.Json;

namespace CTS_NET8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxControlController : ControllerBase
    {
        public ContextDB _connectionString;
        Reply _Reply = new Reply();
        TokenGenerator _ITokens = new TokenGenerator();
        SqlController _sqlController = new SqlController();
        QuerySqlViewModel _info = new QuerySqlViewModel();
        public string query = string.Empty;
        UserController userController = new UserController();
        SedeController sedeController = new SedeController();
        Dictionary<string, object> datos = new Dictionary<string, object>();
        string reply = "";

        public BoxControlController()
        {
            _connectionString = new ContextDB();
        }

        #region Metodo para validar las cajas abiertas de un usuario
        //GET Api/BoxControl/ValidarCaja
        /// <summary>
        /// Consulta - Validar existencia de caja abierta para usuario
        /// </summary>
        /// <param name="user">Variable  que corresponde al identificador del usuario</param>
        [HttpGet]
        [Route("Api/BoxControl/ValidarCaja")]
        public async Task<Reply> ValidateCashRegister(int user, int IdSede)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    connection.Open();
                    var hoy = DateTime.Now.Date;
                    DateTime ahora = DateTime.Now;
                    string horaParaSQL = ahora.ToString("HH:mm:ss");
                    Reply UsuarioGestion =await userController.ConsultUserId(user);
                    dynamic UsuarioGestion2 = UsuarioGestion.Data;
                    string query = _sqlController.ConsultCashRegistersByUser();
                    var parameters = new
                    {
                        user = user,
                        fecha = hoy
                    };
                    var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(query, parameters);
                    if (resultado == null)
                    {
                        datos.Add("Open", false);
                        return new Reply
                        {
                            Ok = false,
                            Message = "Para poder continuar realice la apertura de una caja.",
                            Data = datos
                        };
                    }

                    if (resultado != null && resultado.IdSede != IdSede)
                    {
                        Reply SedeApertura = await sedeController.ConsultSedeId(resultado.IdSede);
                        dynamic SedeApertura2 = SedeApertura.Data;
                        datos.Add("usuarioCierre", resultado.Fecha.ToString("dd/MM/yyyy") + " " + resultado.HoraApertura + " " + UsuarioGestion2[0].NombreCompleto + "" + SedeApertura2[0].NombreSede);
                        datos.Add("SedeAbierta", resultado.IdSede);
                        datos.Add("Open", true);
                        datos.Add("IdCaja", resultado.IdRegistro);
                        return new Reply
                        {
                            Ok = false,
                            Message = "El usuario tiene una caja abierta en otra sede. Debe cerrar la caja y volver a abrir para poder continuar",
                            Data = datos
                        };
                    }


                    string queryUltimaCaja = _sqlController.ConsultLastCashRegistersByUser();
                    var parameters2 = new
                    {
                        user = user
                    };

                    var cajaAnterior = await connection.QueryFirstOrDefaultAsync<dynamic>(queryUltimaCaja, parameters2);          

                    if (cajaAnterior != null && cajaAnterior.Fecha.ToString("dd/MM/yyyy") != hoy.ToString("dd/MM/yyyy") && cajaAnterior.Estado == 117)
                    {
                        Reply SedeApertura = await sedeController.ConsultSedeId(cajaAnterior.IdSede);
                        dynamic SedeApertura2 = SedeApertura.Data;
                        datos.Add("usuarioCierre", cajaAnterior.Fecha.ToString("dd/MM/yyyy") + " " + cajaAnterior.HoraApertura + " " + UsuarioGestion2[0].NombreCompleto + "" + SedeApertura2[0].NombreSede);
                        datos.Add("SedeAbierta", cajaAnterior.IdSede);
                        datos.Add("Open", true);
                        datos.Add("IdCaja", cajaAnterior.IdRegistro);
                        return new Reply
                        {
                            Ok = false,
                            Message = "Debe cerrar la caja del día anterior antes de abrir una nueva.",
                            Data = datos
                        };
                    }

                    else
                    {
                        Reply SedeApertura = await sedeController.ConsultSedeId(resultado.IdSede);
                        dynamic SedeApertura2 = SedeApertura.Data;
                        datos.Add("usuarioCierre", resultado.Fecha.ToString("dd/MM/yyyy") + " " + resultado.HoraApertura + " " + UsuarioGestion2[0].NombreCompleto + " " + SedeApertura2[0].NombreSede);
                        datos.Add("SedeAbierta", resultado.IdSede);
                        datos.Add("Open", true);
                        datos.Add("IdCaja", resultado.IdRegistro);
                        return new Reply
                        {
                            Ok = true,
                            Message = "Validación completada correctamente.",
                            Data = datos
                        };
                    }
                }
                catch (Exception ex)
                {
                    _Reply.Ok = false;
                    _Reply.Data = null;
                    _Reply.Message = $"Error: {ex.Message}";
                    _Reply.Status = 500;
                    return _Reply;
                }
            }
        }
        #endregion


    }
}
