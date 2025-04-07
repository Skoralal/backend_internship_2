namespace InternalApi.Models
{
    public class NetOptions
    {
        public string ApiKey { get; set; }
        public string BaseURL { get; set; }
        public int gRPCPort { get; set; }
        public int HTTPPort { get; set; }
    }
}
