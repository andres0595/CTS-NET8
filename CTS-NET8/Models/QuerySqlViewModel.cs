namespace CTS_NET8.Models
{
    public class QuerySqlViewModel
    {
        public string? tabla { get; set; }
        public string[]? valores { get; set; }
        public string[]? condiciones { get; set; }
        public string[]? tablas { get; set; }
        public string[]? idenTabla { get; set; }
        public string[]? join { get; set; }
        public object[]? inserts { get; set; }
        public int[]? typeCondition { get; set; }
        public int eval { get; set; }
        public int[]? types { get; set; }
        public int igualador { get; set; }
        public int typeInner { get; set; }
    }
}
