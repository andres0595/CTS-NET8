namespace CTS_NET8.Models
{
    public class UserSystem
    {
        public int id { get; set; }
        public int perfil { get; set; }
        public string? nombreCompleto { get; set; }
        public string? documento { get; set; }
        public string? correo { get; set; }
        public string? usuario { get; set; }    
        public string? fechaNacimiento { get; set; }
        public string? contrasena { get; set; }
        public int activo { get; set; }
        public int idsede { get; set; }
        public string? NombrePerfil { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime UltimaFechaingreso { get; set; }
        public string[]? SedesArray { get; set; }     
        public Boolean EstadoBool { get; set; }
        public int[]? ModuloArray { get; set; }
        public int[]? Permisos { get; set; }
        public string? UsuarioAthenea { get; set; }
        public int idUsuario_creacion { get; set; }
        public string? Usuario_Encriptado { get; set; }
        public string? Firma { get; set; }
        public int Idtipodocumento { get; set; }
        public string? fechaFirma { get; set; }
    }
}
