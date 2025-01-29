using CTS_NET8.Models;

namespace CTS_NET8.Connection
{
    public class InterfaceConfig
    {
        static internal string vitaleaConexionDB;
        static internal string ConnectionStorage;
        static internal string ContainerBlob;
        public string secretKeyJWT;
        public string expirationJWT;
        public string issuerJWT;
        public string audienceJWT;
        public string Environment { get; private set; }
        public string keySecurityEncryptionPassUsers;
        public string UrlGeneral;
        public string UrlRecuperacion;
        public void InitializeConfig()
        {
            var constructor = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuracion = constructor.Build();
            secretKeyJWT = configuracion.GetSection("Configuracion").GetSection("webAPI").GetSection("secretKeyJWT").Value;
            expirationJWT = configuracion.GetSection("Configuracion").GetSection("webAPI").GetSection("expirationJWT").Value;
            issuerJWT = configuracion.GetSection("Configuracion").GetSection("webAPI").GetSection("issuerJWT").Value;
            audienceJWT = configuracion.GetSection("Configuracion").GetSection("webAPI").GetSection("audienceJWT").Value;
            ConnectionStorage = configuracion.GetSection("DataBlobStorage").GetSection("ConnectionStorage").Value;
            keySecurityEncryptionPassUsers = configuracion.GetSection("Configuracion").GetSection("webAPI").GetSection("keySecurityEncryptionPassUsers").Value;


            bool activarAmbientePruebas = true; //SIEMPRE CAMBIAR A FALSE PARA CONECTAR AMBIENTE PRODUCCION    
            if (activarAmbientePruebas)
            {
                vitaleaConexionDB = configuracion.GetSection("ConexionesDB").GetSection("vitaleaConexionTestDB").Value;              
                ContainerBlob = configuracion.GetSection("DataBlobStorage").GetSection("BlobPruebas").Value;
                Environment = EmunSystem.ambienteDesarrollo.GetDescription();
                UrlGeneral = "https://crm-frontend-vitalea-pruebas.azurewebsites.net/Login";
                UrlRecuperacion = "https://crm-frontend-vitalea-pruebas.azurewebsites.net/cambiar-contrasena/";

            }
            else
            {
                vitaleaConexionDB = configuracion.GetSection("ConexionesDB").GetSection("vitaleaConexionDB").Value;
                ContainerBlob = configuracion.GetSection("DataBlobStorage").GetSection("BlobProduccion").Value;
                Environment = EmunSystem.ambienteProduccion.GetDescription();
                UrlGeneral = "https://cts.vitalea.com.co/Login";
                UrlRecuperacion = "https://cts.vitalea.com.co/cambiar-contrasena/";
            }

        }
    }
}
