using System.Data.SqlClient;

namespace CTS_NET8.Connection
{
    public class ContextDB
    {
        public SqlConnection ConnectBD()
        {
            return new SqlConnection(InterfaceConfig.vitaleaConexionDB);
        }

        //public SqlConnection ConnectBDVioleta()
        //{
        //    return new SqlConnection(InterfaceConfig.violetaConexionDB);
        //}
    }
}
