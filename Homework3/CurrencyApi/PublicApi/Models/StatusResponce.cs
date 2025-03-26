namespace Fuse8.BackendInternship.PublicApi.Models
{
    // StatusResponce myDeserializedClass = JsonConvert.DeserializeObject<StatusResponce>(myJsonResponse);
    public class Grace
    {
        public int total { get; set; }
        public int used { get; set; }
        public int remaining { get; set; }
    }

    public class Month
    {
        public int total { get; set; }
        public int used { get; set; }
        public int remaining { get; set; }
    }

    public class Quotas
    {
        public Month month { get; set; }
        public Grace grace { get; set; }
    }

    public class StatusResponce
    {
        public long account_id { get; set; }
        public Quotas quotas { get; set; }
    }


}
