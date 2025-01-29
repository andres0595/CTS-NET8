namespace CTS_NET8.Models
{
    public class Sedes
    {
        public int id { get; set; }
        public string? nombre { get; set; }
        public int IdCiudad { get; set; }
        public int activo { get; set; }
        public Boolean EstadoBool { get; set; }
        public string? Ciudad { get; set; }
        public int IdDepartamento { get; set; }
        public string? CodAthenea { get; set; }
        public string? CodCredibanco { get; set; }
        public int idUsuario { get; set; }
        public int idSedeLis { get; set; }
    }
}
