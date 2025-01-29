using CTS_NET8.Configurations;
using CTS_NET8.Connection;
using CTS_NET8.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace CTS_NET8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController : ControllerBase
    {
        public ContextDB _connectionString;
        Reply _Reply = new Reply();
        TokenGenerator _ITokens = new TokenGenerator();
        SqlController _sqlController = new SqlController();
        QuerySqlViewModel _info = new QuerySqlViewModel();
        public string query = string.Empty;

        public GenericController()
        {
            _connectionString = new ContextDB();
        }

        #region Metodo para consultar los departamentos
        [HttpGet]
        [Route("ConsultarDeptos")]
        public async Task<Reply> ConsultDepartments()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Departamentos";
                    _info.valores = new string[2] { "IdDepartamento", "Nombre" };
                    _info.condiciones = new string[1] { "Activo" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1 });
                    if (DeptoRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay departamentos registrados";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoRetorno.ToArray();
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

        #region Metodo para consultar por ID de departamento las ciudades
        [HttpGet]
        [Route("ConsultarCiudadesDeptos")]
        public async Task<Reply> ConsultCityDepartments(int idDepto)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Ciudades";
                    _info.valores = new string[3] { "IdCiudad AS Id", "Nombre", "IdHis" };
                    _info.condiciones = new string[2] { "Activo", "IdDepartamentp" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1, IdDepartamentp = idDepto });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay ciudades para el departamento seleccionado";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar por ID de ciudad las localidades
        [HttpGet]
        [Route("ConsultarLocalidadesCiudad")]
        public async Task<Reply> ConsultLocalitiesCity(int IdCiudad)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Localidades";
                    _info.valores = new string[2] { "IdLocalidad AS Id", "NombreLocalidad AS Nombre" };
                    _info.condiciones = new string[2] { "Activo", "IdCiudad" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1, IdCiudad = IdCiudad });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay localidades para la ciudad seleccionada";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar por ID de ciudad las localidades
        [HttpGet]
        [Route("ConsultarBarriosLocalidad")]
        public async Task<Reply> ConsultNeighborhoods(int IdLocalidad)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Barrios";
                    _info.valores = new string[2] { "IdBarrio AS Id", "NombreBarrio AS Nombre" };
                    _info.condiciones = new string[2] { "Activo", "IdLocalidad" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1, IdLocalidad = IdLocalidad });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay barrios para la localidad seleccionada";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar los tipos de afiliacion
        [HttpGet]
        [Route("ConsultarTipoAfiliacion")]
        public async Task<Reply> ConsultAffiliationType()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "TipoAfiliacion";
                    _info.valores = new string[2] { "IdTipAfil AS Id", "TipoAfiliacion" };
                    _info.condiciones = new string[1] { "Activo" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1 });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay tipos de afiliación registrados";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar los estratos
        [HttpGet]
        [Route("ConsultarEstratos")]
        public async Task<Reply> ConsultStratum()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Estratos";
                    _info.valores = new string[2] { "IdEstrato AS Id", "Nombre" };
                    _info.condiciones = new string[1] { "Activo" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1 });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay estratos registrados";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar los estratos
        [HttpGet]
        [Route("ConsultarGeneros")]
        public async Task<Reply> ConsultGender()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Sexo";
                    _info.valores = new string[2] { "IdSexo AS Id", "Nombre" };
                    _info.condiciones = new string[1] { "Activo" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1 });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay generos registrados";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar los estratos
        [HttpGet]
        [Route("ConsultarTipoDocumento")]
        public async Task<Reply> ConsultDocumentType()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "TiposDocumento";
                    _info.valores = new string[3] { "IdTipoDocumento AS Id", "Abreviatura +' - '+  Nombre AS Nombre", "Abreviatura" };
                    _info.condiciones = new string[1] { "Activo" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1 });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay tipos de documento registrados";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar estado civil
        [HttpGet]
        [Route("ConsultarEstadoCivil")]
        public async Task<Reply> ConsultCivilStatus()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "EstadoCivil";
                    _info.valores = new string[3] { "IdEstadoCivil AS Id", "Nombre", "IdHis" };
                    _info.condiciones = new string[1] { "Activo" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1 });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay registros";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar paises
        [HttpGet]
        [Route("ConsultarPaises")]
        public async Task<Reply> ConsultCountries()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Paises";
                    _info.valores = new string[2] { "IdPais AS Id", "Nombre" };
                    _info.condiciones = new string[1] { "Activo" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1 });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay registros";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar EPS
        [HttpGet]
        [Route("ConsultarEps")]
        public async Task<Reply> ConsultEps()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Eps";
                    _info.valores = new string[3] { "IdEps AS Id", "Nombre", "IdHis" };
                    _info.condiciones = new string[1] { "Activo" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1 });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay registros";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar los estados de la venta
        [HttpGet]
        [Route("ConsultarEstadosVenta")]
        public async Task<Reply> ConsultStateSales()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    query = _sqlController.ConsultStateSales();
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query);
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay registros";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar por categoria las opciones del perfil
        [HttpGet]
        [Route("ConsultarOpcionesCategoriaPerfil")]
        public async Task<Reply> ConsultOptionsCategoryProfile(int id, int perfil)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    query = _sqlController.ConsultOptions();
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { id, perfil });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay opciones para la categoria seleccionada";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar los usuarios con perfil de asesor comercial
        [HttpGet]
        [Route("ConsultarUsuariosPerfil")]
        public async Task<Reply> ConsultAdviser(int perfil)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    query = _sqlController.ConsultAdviser();
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { perfil });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay registros";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo apra consultar los estados de la transaccion
        [HttpGet]
        [Route("ConsultarEstadosTransaccion")]
        public async Task<Reply> ConsultStateTransaction()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "EstadoTransaccion";
                    _info.valores = new string[2] { "IdEstadoTran", "Estado" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1 });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay registros";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar por categoria las opciones del perfil
        [HttpGet]
        [Route("ConsultarOpcionesCategoria")]
        public async Task<Reply> ConsultOptionsCategory(int id)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Opciones";
                    _info.valores = new string[2] { "IdOpc", "Opcion" };
                    _info.condiciones = new string[1] { "IdCatOpc" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { IdCatOpc = id });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay opciones para la categoria seleccionada";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar nacionalidades
        [HttpGet]
        [Route("ConsultarNacionalidad")]
        public async Task<Reply> ConsultNationality()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Nacionalidad";
                    _info.valores = new string[2] { "IdNacionalidad", "Nacionalidad" };
                    _info.condiciones = new string[1] { "Activo" };
                    query = _sqlController.Selectotable(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query, new { Activo = 1 });
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay registros";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar estados de la solicitud
        [HttpGet]
        [Route("ConsultarEstadosSolicitud")]
        public async Task<Reply> ConsultStateRequest()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    query = _sqlController.ConsultStateRequest();
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query);
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay registros";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para inactivar registros
        [HttpDelete]
        [Route("InactivarRegistros")]
        public async Task<Reply> InactivateRecords(int id, int bandera, int activo, int idusuario)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    // Parámetros para el procedimiento almacenado
                    var parameters = new DynamicParameters();
                    parameters.Add("@id", id, DbType.Int32);
                    parameters.Add("@bandera", bandera, DbType.Int32);
                    parameters.Add("@activo", activo, DbType.Int32);
                    parameters.Add("@idusuario", idusuario, DbType.Int32);
                    parameters.Add("@mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

                    // Ejecuta el procedimiento almacenado
                    await connection.ExecuteAsync("sp_Inactivar_Registros", parameters, commandType: CommandType.StoredProcedure);

                    // Recupera el mensaje de salida
                    string mensaje = parameters.Get<string>("@mensaje");

                    // Configura la respuesta
                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = null;
                    _Reply.Message = mensaje;

                    return _Reply;
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones
                    _Reply.Ok = false;
                    _Reply.Data = null;
                    _Reply.Message = $"Error: {ex.Message}";
                    _Reply.Status = 500;

                    return _Reply;
                }
            }
        }
        #endregion

        #region Metodo para actualizar el contenido HTML de los productos
        [HttpGet]
        [Route("CrearContenidoProcedimientos")]
        public async Task<Reply> CreateContentProcedures(ContenidoWeb _contenido)
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    query = _sqlController.CreateContentsProcedures(_contenido.CategoriaProcedimiento);
                    var response = await connection.ExecuteAsync(query, new { ContenidoHTML = _contenido.Contenido_Html, IdProcedimiento = _contenido.IdProcedimiento });
                    if (response == null)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Status = 400;
                        _Reply.Message = $"Error en actualizar registro";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = null;
                    _Reply.Message = response.ToString();
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

        #region Metodo para consultar los parametros de marcacion de procedimientos
        [HttpGet]
        [Route("ConsultarParametrosMarcaciones")]
        public async Task<Reply> ConsultMarkingsParameters()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tabla = "Opciones";
                    _info.valores = new string[3] { "Codigo", "Opcion", "IdCatOpc" };
                    _info.condiciones = new string[2] { "17", "18" };
                    _info.join = new string[1] { "IdCatOpc" };
                    _info.types = new int[1] { 0 };
                    query = _sqlController.selectIn(_info);
                    var DeptoCityRetorno = await connection.QueryAsync<dynamic>(query);
                    if (DeptoCityRetorno.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay registros";
                        return _Reply;
                    }

                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = DeptoCityRetorno.ToArray();
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

        #region Metodo para consultar indicativos
        [HttpGet]
        [Route("ConsultarIndicativos")]
        public async Task<Reply> Consultindicatives()
        {
            List<Dictionary<string, object>> listaDeDatos = new List<Dictionary<string, object>>();
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    await connection.OpenAsync();
                    _info.tablas = new string[2] { "Indicativos", "Paises" };
                    _info.idenTabla = new string[2] { "I", "P" };
                    _info.join = new string[2] { "IdPais", "IdPais" };
                    _info.valores = new string[3] { "I.IdRegistro", "I.Cod_Indicativo", "P.Nombre" };
                    _info.eval = 1;
                    _info.igualador = 0;
                    _info.condiciones = new string[1] { "Activo" };


                    string query = _sqlController.InnerJoin(_info);
                    var parameters = new { Activo = 1 };
                    // Utilizar Query para obtener una colección de resultados
                    var resultados = connection.Query<dynamic>(query, parameters);
                    if (resultados.Count() == 0)
                    {
                        _Reply.Ok = false;
                        _Reply.Data = null;
                        _Reply.Message = $"No hay registros";
                        return _Reply;
                    }

                    foreach (var resultado in resultados)
                    {
                        // Puedes usar un diccionario anidado o una lista de diccionarios para múltiples filas
                        var registro = new Dictionary<string, object>
                            {
                                { "IdRegistro", resultado.IdRegistro },
                                { "Cod_Indicativo", resultado.Cod_Indicativo },
                                { "Pais", resultado.Nombre }
                            };
                        // Agregar el registro a una lista o a otra estructura de datos
                        listaDeDatos.Add(registro);
                    }
                    _Reply.Ok = true;
                    _Reply.Status = 200;
                    _Reply.Data = listaDeDatos;
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
