using CTS_NET8.Configurations;
using CTS_NET8.Connection;
using CTS_NET8.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;

namespace CTS_NET8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SedeController : ControllerBase
    {
        public ContextDB _connectionString;
        Reply _Reply = new Reply();
        TokenGenerator _ITokens = new TokenGenerator();
        SqlController _sqlController = new SqlController();
        QuerySqlViewModel _info = new QuerySqlViewModel();
        public string query = string.Empty;

        public SedeController()
        {
            _connectionString = new ContextDB();
        }

        #region Metodo para consultar sedes por el ID
        [HttpGet]
        [Route("ConsultarsedeId")]
        public async Task<Reply> ConsultSedeId(int id)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    query = "SELECT IdSede,NombreSede,IdCiudad,SL.IdSedeLIS FROM Sedes S LEFT JOIN Sedes_Lis_Homologacion SL ON S.IdSede=SL.IdSedeCTS WHERE IdSede=@id";
                    var SedesRetorno = await connection.QueryAsync<dynamic>(query, new { id });
                    if (SedesRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"Sede no existe";
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

        #region Metodo para crear sedes
        [HttpPost]
        [Route("CrearSede")]
        public async Task<Reply> CreateSede(Sedes sede)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    int? idSede = null;
                    // Lógica para una nueva sede
                    if (sede.id == 0)
                    {
                        // Verificar si ya existe una sede con el mismo nombre
                        query = _sqlController.ValidateSites();
                        var existeSede = connection.QueryFirstOrDefault<int?>(query, new { NombreSede = sede.nombre });
                        // No existe, insertar nueva sede
                        if (existeSede == null)
                        {
                            var insertQuery = _sqlController.CreateOrUpdateSites("Crear");
                            idSede = connection.QuerySingle<int>(insertQuery, new
                            {
                                IdCiudad = sede.IdCiudad,
                                IdAthenea = sede.CodAthenea,
                                NombreSede = sede.nombre,
                                Activo = 1,
                                CodCredibanco = sede.CodCredibanco,
                                IdUsuario = sede.idUsuario
                            });

                            _Reply.Ok = true;
                            _Reply.Status = 200;                            
                            _Reply.Data = idSede;
                            _Reply.Message = "Sede registrada exitosamente";
                            return _Reply;
                        }
                        else
                        {
                            _Reply.Ok = false;
                            _Reply.Status = 400;
                            _Reply.Data = null;
                            _Reply.Message = "Ya existe una sede con este nombre";
                            return _Reply;
                        }
                    }
                    else
                    {
                        var updateQuery = _sqlController.CreateOrUpdateSites("Editar");
                        connection.Execute(updateQuery, new
                        {
                            IdSede = sede.id,
                            NombreSede = sede.nombre,
                            IdCiudad = sede.IdCiudad,
                            CodCredibanco = sede.CodCredibanco,
                            IdUsuario = sede.idUsuario
                        });

                        _Reply.Ok = true;
                        _Reply.Status = 200;
                        _Reply.Data = idSede;
                        _Reply.Message = "Sede modificada exitosamente";
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
        }
        #endregion

        #region Metodo para inactivar las sedes
        [HttpDelete]
        [Route("InactivarSede")]
        public async Task<Reply>InactivarSede(int id, int activo, int idUsuario)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Sedes";
                    _info.valores = new string[2] { "Activo", "IdUsuario" };
                    _info.condiciones = new string[1] { "IdSede" };
                    query = _sqlController.ActualizarTabla(_info);
                    await connection.ExecuteAsync(query, new { Activo= activo, IdUsuario = idUsuario, IdSede= id });

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data =null;
                    _Reply.Message = activo==0? "Sede inactivada exitosamente": "Sede activada exitosamente";
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

        #region  Metodo para consultar sedes
        [HttpGet]
        [Route("Consultarsede")]
        public async Task<Reply> Consultarsede()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    query = _sqlController.ConsultSites();
                    var SedesRetorno = await connection.QueryAsync<dynamic>(query);
                    if (SedesRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay sedes registradas";
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
    }
}
