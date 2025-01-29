namespace CTS_NET8.Models
{
    public class UserLogin
    {
        public string? usuario { get; set; }
        public string? contrasena { get; set; }
        public int idSede { get; set; }
        public string? documento { get; set; }
    }

    public class UserDataResponse
    {
        public int Flag { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public object DatosUsuario { get; set; }
    }
}
