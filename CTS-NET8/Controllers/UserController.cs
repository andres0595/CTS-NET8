using CTS_NET8.Configurations;
using CTS_NET8.Connection;
using CTS_NET8.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;

namespace CTS_NET8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ContextDB _connectionString;
        ServiceMailNotification servicioCorreoNotificacion = new ServiceMailNotification();
        Reply _Reply = new Reply();
        SqlController _sqlController = new SqlController();
        QuerySqlViewModel _info = new QuerySqlViewModel();
        private InterfaceConfig _InterfaceConfig;
        public string query = string.Empty;

        public UserController()
        {
            _connectionString = new ContextDB();
            _InterfaceConfig = new InterfaceConfig();
            _InterfaceConfig.InitializeConfig();
        }

        #region Metodo para crear los usuarios
        [HttpPost]
        [Route("CreateOrUpdateUser")]
        public async Task<Reply> CreateOrUpdateUser(UserSystem userSystem)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    int idUsuario;
                    query = @"SELECT TOP 1
                    CASE
                        WHEN NombreUsuario = @Usuario THEN 'Ya existe un usuario registrado con ese nombre'
                        WHEN Documento = @Documento THEN 'Ya se encuentra registrado un usuario con ese número de documento'
                        WHEN Correo = @Correo THEN 'Ya se encuentra registrado un usuario con ese correo'
                    END AS Conflicto
                  FROM Usuarios
                  WHERE NombreUsuario = @Usuario
                     OR Documento = @Documento
                     OR Correo = @Correo;";
                    // Validaciones combinadas
                    string conflicto = connection.QueryFirstOrDefault<string>(query, new { Usuario = userSystem.usuario, Documento = userSystem.documento, Correo = userSystem.correo });

                    if (!string.IsNullOrEmpty(conflicto))
                    {
                        _Reply.Ok = false;
                        _Reply.Status = 500;
                        _Reply.Data = new { Flag = 2, idusuario = 0};
                        _Reply.Message = conflicto;
                        return _Reply;
                    }

                    // Crear nuevo usuario
                    if (userSystem.id == 0)
                    {
                        query = @"INSERT INTO Usuarios (
                        IdPerfil, NombreCompleto, Documento, Correo, NombreUsuario, Contrasena,
                        FechaNacimiento, FechaCreacion, UsuarioAthenea, Activo, IdUsuario_Creacion,
                        Usuario_Encriptado, Firma, Idtipodocumento)
                      VALUES (
                        @Perfil, @NombreCompleto, @Documento, @Correo, @Usuario, @Contrasena,
                        @FechaNaci, @FechaCreacion, 
                        CASE WHEN @UsuarioAthenea IS NULL OR @UsuarioAthenea = '' THEN NULL ELSE @UsuarioAthenea END,
                        @Activo, @IdUsuarioCreacion, @UsuarioEncriptado, @Firma, @IdTipoDocumento);
                      SELECT SCOPE_IDENTITY();";
                        idUsuario = connection.QuerySingle<int>(query, new
                        {
                            Perfil = userSystem.perfil,
                            NombreCompleto = userSystem.nombreCompleto,
                            Documento = userSystem.documento,
                            Correo = userSystem.correo,
                            Usuario = userSystem.usuario,
                            Contrasena = userSystem.contrasena,
                            FechaNaci = userSystem.fechaNacimiento,
                            FechaCreacion = DateTime.Now,
                            UsuarioAthenea = userSystem.UsuarioAthenea,
                            Activo = 1,
                            IdUsuarioCreacion = userSystem.idUsuario_creacion,
                            UsuarioEncriptado = userSystem.Usuario_Encriptado,
                            Firma = userSystem.Firma,
                            IdTipoDocumento = userSystem.Idtipodocumento
                        });

                        // Procesar sedes
                        if (userSystem.SedesArray.Length > 0)
                        {
                            ProcesarSedes(connection, userSystem.SedesArray, idUsuario, 1);
                        }
                        EnvioCorreo(userSystem.correo, userSystem.nombreCompleto, userSystem.usuario, userSystem.documento);

                        _Reply.Ok = true;
                        _Reply.Status = 200;
                        _Reply.Data = new { Flag = 1, idusuario = idUsuario };
                        _Reply.Message = "Usuario registrado exitosamente";
                        return _Reply;
                    }
                    else
                    {
                        connection.Execute(
                              @"UPDATE Usuarios
                                      SET IdPerfil = @Perfil,
                                          NombreCompleto = @NombreCompleto,
                                          Documento = @Documento,
                                          Correo = @Correo,
                                          NombreUsuario = @Usuario,
                                          UsuarioAthenea = CASE WHEN @UsuarioAthenea IS NULL OR @UsuarioAthenea = '' THEN NULL ELSE @UsuarioAthenea END,
                                          IdUsuario_Creacion = @IdUsuarioCreacion,
                                          Usuario_Encriptado = @UsuarioEncriptado,
                                          Firma = CASE WHEN @Firma = '' THEN Firma ELSE @Firma END,
                                          Idtipodocumento = @IdTipoDocumento
                                      WHERE IdUsuario = @Id;",
                          new
                          {
                              Id = userSystem.id,
                              Perfil = userSystem.perfil,
                              NombreCompleto = userSystem.nombreCompleto,
                              Documento = userSystem.documento,
                              Correo = userSystem.correo,
                              Usuario = userSystem.usuario,
                              UsuarioAthenea = userSystem.UsuarioAthenea,
                              IdUsuarioCreacion = userSystem.idUsuario_creacion,
                              UsuarioEncriptado = userSystem.Usuario_Encriptado,
                              Firma = userSystem.Firma,
                              IdTipoDocumento = userSystem.Idtipodocumento
                          });

                        // Eliminar y actualizar sedes
                        connection.Execute("DELETE FROM Sedes_x_Usuario WHERE IdUsuario = @IdUsuario;", new { IdUsuario = userSystem.id });
                        if (userSystem.SedesArray.Length > 0)
                        {
                            ProcesarSedes(connection, userSystem.SedesArray, userSystem.id, 1);
                        }

                        _Reply.Ok = true;
                        _Reply.Status = 200;
                        _Reply.Data = new { Flag = 1, idusuario = userSystem.id };
                        _Reply.Message = "Usuario actualizado exitosamente";
                        return _Reply;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        private void ProcesarSedes(SqlConnection connection, string[] sedes, int idUsuario, int activo)
        {
            // Dividir las sedes y procesarlas una por una
            // Iterar por el array de sedes y procesarlas
            foreach (var idSede in sedes)
            {
                connection.Execute(
                    @"INSERT INTO SEDES_X_USUARIO (IdSede, IdUsuario, Activo)
              VALUES (@IdSede, @IdUsuario, @Activo);",
                    new { IdSede = idSede, IdUsuario = idUsuario, Activo = activo });
            }
        }
        #endregion

        #region Metodo para consultar usuarios
        [HttpGet]
        [Route("ConsultarUsuarios")]
        public async Task<Reply> ConsultUser()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    query = _sqlController.ConsultUser();
                    var SedesRetorno = await connection.QueryAsync<dynamic>(query);
                    if (SedesRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay usuarios registrados";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = SedesRetorno.ToArray();
                    _Reply.Message = "Consulta completada con exito";
                    return _Reply;

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

        #region Metodo para inactivar usuarios
        [HttpDelete]
        [Route("InactivarUsuarios")]
        public async Task<Reply> InactiveUser(int id, int activo, int idUsuario)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Usuarios";
                    _info.valores = new string[2] { "Activo", "IdUsuario_Creacion" };
                    _info.condiciones = new string[1] { "IdUsuario" };
                    query = _sqlController.ActualizarTabla(_info);
                    await connection.ExecuteAsync(query, new { Activo = activo, IdUsuario = id, IdUsuario_Creacion = idUsuario });

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = null;
                    _Reply.Message = activo == 0 ? "Usuario inactivado exitosamente" : "Usuario activado exitosamente";
                    return _Reply;
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

        #region Metodo para consultar usuarios por ID
        [HttpGet]
        [Route("UsuariosId")]
        public async Task<Reply> ConsultUserId(int id)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    query = _sqlController.ConsultUserId();
                    var SedesRetorno = await connection.QueryAsync<dynamic>(query, new {id});
                    if (SedesRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay usuarios registrados";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = SedesRetorno.ToArray();
                    _Reply.Message = "Consulta completada con exito";
                    return _Reply;

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

        #region Metodo para notificar creación de usuarios vía email       
        private void EnvioCorreo(string email, string nombreCompleto, string usuario, string password)
        {
            /*Se crea el cuerpo del correo*/
            string body =
                "<body>" +
                "<div>" +
                      "<h1 style = 'color:#30B9F0' >¡Hola!  " + nombreCompleto + " </h1>" +
                      "<h1>Bienvenido a <img style='width: 6rem;' src ='cid:imagen'/></h1>" +
                      "<p> Estas son tus credenciales, Recuerda que la</p>" +
                      "<p>contraseña es temporal y al acceder por</p>" +
                      "<p>primera vez el sistema te solicitará cambiar</p>" +
                      "<p>la contraseña.</p>" +
                      "<hr color = '#E7ED10' size = 3>" +
                      "<div style='width: 19rem;background-color:#F0F0EE;height: 12rem;'>" +
                          "<h3> Credenciales asignadas </h3>" +
            "<p><b>Nombre de usuario: </b> " + usuario + " </p>" +
            "<p><b>Contraseña: </b> " + password + " </p>" +
                           "<p><b>Url de acceso: </b> <a  href =" + _InterfaceConfig.UrlGeneral + ">" + _InterfaceConfig.UrlGeneral + " </a></p><br/><br/>" +
                       "</div>" +
                       "<p>Saludos Cordiales</p>" +
                "</div>" +
                "</body>";
            MailNotification correoConfig;
            correoConfig = servicioCorreoNotificacion.ObtenerCorreoNotificaciones();

            /*Linea para definir el formato del correo*/
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
            /******************************************/
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(correoConfig.Correo, "Registro Usuario");
            mail.To.Add(new MailAddress(email));
            mail.Subject = "Mensaje Bienvenida";
            mail.BodyEncoding = Encoding.UTF8;
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            mail.IsBodyHtml = true;
            mail.Body = body;

            /*Se adjunta el logo de vitalea*/           
            string imagePathF = @"Resources/Images/vitalea2.png";//aqui la ruta de tu imagen
            LinkedResource logo = new LinkedResource(imagePathF, MediaTypeNames.Image.Jpeg);
            logo.ContentId = "imagen";
            htmlView.LinkedResources.Add(logo);
            mail.AlternateViews.Add(htmlView);
            /*******************************/

            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(correoConfig.Correo, correoConfig.Clave);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(mail);
            //return "Ok";
        }
        #endregion



    }
}
