using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class ApiResponse
    {
            public Meta meta { get; set; }
            public Dictionary<string, Currency> data { get; set; }
    }

    public class Meta
    {
        public DateTime last_updated_at { get; set; }
    }


    public class Currency
    {
        public string code { get; set; }
        public double value { get; set; }
        public void Deconstruct(out string code, out double value)
        {
            code = this.code;
            value = this.value;
        }
    }
}
