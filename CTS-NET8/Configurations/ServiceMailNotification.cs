using CTS_NET8.Connection;
using CTS_NET8.Models;
using Dapper;
using System.Data.SqlClient;

namespace CTS_NET8.Configurations
{
    public class ServiceMailNotification
    {
        private ContextDB _connectionString;
        SqlController sqlController = new SqlController();

        public ServiceMailNotification()
        {
            _connectionString = new ContextDB();
        }

        public MailNotification ObtenerCorreoNotificaciones()
        {
            using (SqlConnection connection = _connectionString.ConnectBD())
            {
                try
                {
                    connection.Open();
                    QuerySqlViewModel infoC = new QuerySqlViewModel
                    {
                        tabla = "CorreoNotificaciones",
                        valores = new string[] { "Correo", "Clave" }
                    };

                    string query = sqlController.Selectotable(infoC);

                    return connection.QueryFirstOrDefault<MailNotification>(query);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
