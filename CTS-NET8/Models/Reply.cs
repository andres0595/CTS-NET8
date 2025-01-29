namespace CTS_NET8.Models
{
    public class Reply
    {
        public bool Ok { get; set; }
        public string? Message { get; set; }
        public int Status { get; set; }
        public object? Data { get; set; }
    }
}
