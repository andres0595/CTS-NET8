using CTS_NET8.Configurations;
using CTS_NET8.Connection;
using CTS_NET8.Models;
using CTS_NET8.Security;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace CTS_NET8.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public ContextDB _connectionString;
        Reply _Reply = new Reply();
        TokenGenerator _ITokens = new TokenGenerator();
        SqlController _sqlController = new SqlController();
        QuerySqlViewModel _info = new QuerySqlViewModel();

        public LoginController()
        {
            _connectionString = new ContextDB();
        }

        [HttpPost]
        [Route("AutenticarUsuarios")]
        public async Task<Reply> LoginAsync(UserLogin userLogin)
        {
            try
            {
                using (SqlConnection connection = _connectionString.ConnectBD())
                {
                    var queryIdUsuario = _sqlController.ConsultUserData();
                    var usuarioData = await connection.QuerySingleOrDefaultAsync<(int IdUsuario, DateTime? UltimaFechaIngreso, string UsuarioEncriptado, string documento, int IdPerfil,
                        string UsuarioIntegracion, string Rol, string Usuario)>(queryIdUsuario, new { userLogin.usuario });

                    if (usuarioData == default)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"3, {(string.IsNullOrEmpty(usuarioData.UsuarioEncriptado) ? "Sin usuario" : usuarioData.UsuarioEncriptado)}";
                        return _Reply;
                    }

                    string loginStatus = usuarioData.UltimaFechaIngreso == null ? "0" : "1";
                    if (loginStatus == "0")
                        userLogin.contrasena = Encrypt.Encryption(usuarioData.documento);
                    else
                        userLogin.contrasena = Encrypt.Encryption(userLogin.contrasena);

                    var queryEstadoUsuario = _sqlController.ConsultUserLogin();
                    var estado = await connection.QuerySingleOrDefaultAsync<int?>(queryEstadoUsuario, new { userLogin.usuario, userLogin.contrasena });

                    var queryEstadoSede = _sqlController.ConsultAttentionCenterData();
                    var estadoSede = await connection.QuerySingleOrDefaultAsync<int?>(queryEstadoSede, new { userLogin.idSede });

                    var modulosUsuario = await connection.QueryAsync<int>("EXEC sp_Modulos_UsuarioPerfil @usuario", new { userLogin.usuario });
                    var modulosUsuarioRetorno = await connection.QueryAsync<dynamic>("EXEC sp_Modulos_UsuarioPerfil @usuario", new { userLogin.usuario });
                    var menuCount = modulosUsuario.Count();

                    if (estado == null)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = EmunSystem.UsuarioNoValido.GetDescription();
                        return _Reply;
                    }

                    if (estado != 1)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = EmunSystem.UsuarioInactivo.GetDescription();
                        return _Reply;
                    }

                    if (estadoSede != 1)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = EmunSystem.SedeInactiva.GetDescription();
                        return _Reply;
                    }

                    if (menuCount == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = EmunSystem.UsuarioSinModulos.GetDescription();
                        return _Reply;
                    }

                    if (loginStatus == "0")
                    {
                        _Reply.Ok = true;
                        _Reply.Data = new { Flag = 0, UsuarioEncriptado = usuarioData.UsuarioEncriptado };
                        _Reply.Status = 200;
                        _Reply.Message = EmunSystem.AutenticacionPrimeraVez.GetDescription();
                        return _Reply;
                    }



                    //var insertLog = "INSERT INTO LogIngresoSistema VALUES(@IdUsuario, @idSede, DATEADD(HOUR, -5, GETDATE()))";
                    _info.tabla = "LogIngresoSistema";
                    _info.inserts = new string[3] { "IdUsuario", "IdSede", "FechaRegistro" };
                    string insertLog = _sqlController.InsertDapper(_info);
                    await connection.ExecuteAsync(insertLog, new { IdUsuario = usuarioData.IdUsuario, IdSede = userLogin.idSede, FechaRegistro = DateTime.Now });
                    _info = new QuerySqlViewModel();


                    _info.tabla = "Usuarios";
                    _info.valores = new string[1] { "UltimaFechaIngreso" };
                    _info.condiciones = new string[1] { "IdUsuario" };
                    string updateUltimoIngreso = _sqlController.ActualizarTabla(_info);

                    // var updateUltimoIngreso = "UPDATE Usuarios SET UltimaFechaIngreso = DATEADD(HOUR, -5, GETDATE()) WHERE IdUsuario = @IdUsuario";
                    await connection.ExecuteAsync(updateUltimoIngreso, new { usuarioData.IdUsuario, UltimaFechaIngreso = DateTime.Now });

                    Dictionary<string, string> DatosPaciente = new Dictionary<string, string>
                       {
                        {"IdUsuario",usuarioData.IdUsuario.ToString()},
                        {"IdPerfil",usuarioData.IdPerfil.ToString()},
                        {"UsuarioIntegracion",usuarioData.UsuarioIntegracion},
                        {"Rol",usuarioData.Rol},
                        {"Usuario",usuarioData.Usuario},
                        {"MenuUsuario",JsonConvert.SerializeObject(modulosUsuarioRetorno) }
                       };

                    _Reply.Ok = true;
                    _Reply.Data = new UserDataResponse
                    {
                        Flag = 1,
                        Token = _ITokens.GenerateTokenJwt(userLogin.usuario),
                        RefreshToken = _ITokens.GenerateRefreshTokenJwt(),
                        DatosUsuario = DatosPaciente
                    };
                    _Reply.Status = 200;
                    _Reply.Message = EmunSystem.AutenticacionExitosa.GetDescription();

                    return _Reply;
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

        [HttpGet]
        [Route("ConsultarSedeUser")]
        public async Task<Reply> ConsultSedeUser(string Usuario)
        {
            try
            {
                using (SqlConnection connection = _connectionString.ConnectBD())
                {
                    var queryIdUsuario = "SELECT SU.IdSede as id,S.NombreSede as nombre FROM Sedes_x_Usuario SU INNER JOIN Sedes S ON SU.IdSede=S.IdSede INNER JOIN Usuarios U ON U.IdUsuario=SU.IdUsuario WHERE U.NombreUsuario=@usuario AND S.Activo=1";
                    var SedesUsuarioRetorno = await connection.QueryAsync<dynamic>(queryIdUsuario, new { Usuario });
                    if (SedesUsuarioRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"Usuario sin sedes";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = SedesUsuarioRetorno.ToArray();
                    _Reply.Message = "Sedes consultadas con exito";
                    return _Reply;
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

        [HttpPut]
        [Route("CambiarContrasena")]
        public async Task<string> ActualizarContrasenaAsync(string usuario, string nuevaContrasena)
        {
            try
            {
                using (SqlConnection connection = _connectionString.ConnectBD())
                {
                    bool validador = ValidarFechaUrl(usuario);
                    if (validador == true)
                    {
                        await connection.OpenAsync();
                        _info.tabla = "Usuarios";
                        _info.valores = new string[1] { "1" };
                        _info.condiciones = new string[1] { "Usuario_Encriptado" };
                        // Verificar si el usuario existe
                        var queryUsuario = _sqlController.Selectotable(_info);
                        var existeUsuario = await connection.ExecuteScalarAsync<int>(queryUsuario, new { Usuario_Encriptado=usuario });
                        if (existeUsuario > 0)
                        {
                            // Actualizar contraseña y fecha de ingreso
                            string query = @"
                            UPDATE USUARIOS 
                            SET CONTRASENA = @nuevaContrasena,
                                ULTIMAFECHAINGRESO = DATEADD(HOUR,-5,GETDATE())
                            WHERE Usuario_Encriptado = @usuario";

                            await connection.ExecuteAsync(query, new { usuario, nuevaContrasena });
                            return "Tu cambio de contraseña ha sido exitoso";
                        }
                        else
                        {
                            return "Tu usuario no registra en el sistema. Por favor verifica los datos";
                        }
                    }
                    else
                    {
                        return "Lo sentimos la url que intentas usar a caducado";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private bool ValidarFechaUrl(string token)
        {
            bool validate = true;
            _info.tabla = "Usuarios";
            _info.valores = new string[1] { "Fecha_Vencimiento_Url" };
            _info.condiciones = new string[1] { "Usuario_Encriptado" };
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(_sqlController.Selectotable(_info), connection);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("Usuario_Encriptado", token);
                    SqlDataReader sqldr = cmd.ExecuteReader();
                    DateTime date = new DateTime();
                    while (sqldr.Read())
                    {
                        date = Convert.ToDateTime(sqldr["Fecha_Vencimiento_Url"]);
                    }
                    sqldr.Close();

                    var minutes = (DateTime.Now - date).TotalMinutes;
                    validate = minutes > 5 ? false : true;
                }
                catch (Exception ex)
                {
                    return false;
                }

                return validate;
            }

        }
    }
}
