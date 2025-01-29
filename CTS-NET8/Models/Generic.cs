namespace CTS_NET8.Models
{
    public class Generic
    {
        public int Codigo { get; set; }
        public string? Opcion { get; set; }
        public int IdCatOpc { get; set; }
    }

    public class ContenidoWeb
    {
        public int IdProcedimiento { get; set; }
        public string? CategoriaProcedimiento { get; set; }
        public string? Contenido_Html { get; set; }
    }

    public class Nacionalidades
    {
        public int idNacionalidad { get; set; }
        public string? Nacionalidad { get; set; }
    }

    public class Pais
    {
        public int id { get; set; }
        public string pais { get; set; }
    }
    public class Ciudades
    {
        public int id { get; set; }
        public string ciudad { get; set; }
        public string Codigo { get; set; }
        public int IdHis { get; set; }
    }

    public class Depto
    {
        public int id { get; set; }
        public string depto { get; set; }
    }

    public class Localidades
    {
        public int id { get; set; }
        public string localidad { get; set; }
    }

    public class Barrios
    {
        public int id { get; set; }
        public string barrio { get; set; }
    }

    public class TipoAfil
    {
        public int id { get; set; }
        public string tipoafiliacion { get; set; }
    }

    public class Estrato
    {
        public int id { get; set; }
        public string estrato { get; set; }
    }

    public class Genero
    {
        public int id { get; set; }
        public string genero { get; set; }
        public string abreviatura { get; set; }
    }

    public class TipoDocumento
    {
        public int id { get; set; }
        public string tipodocumento { get; set; }
        public string abreviatura { get; set; }
    }

    public class EstadoCivil
    {
        public int id { get; set; }
        public string estadoCiv { get; set; }
        public int IdHis { get; set; }
    }

    public class Eps
    {
        public int id { get; set; }
        public string eps { get; set; }
        public int IdHis { get; set; }
    }
}
