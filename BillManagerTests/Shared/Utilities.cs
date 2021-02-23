using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace BillManagerTests.Shared
{
    public static class Utilities
    {
        public static StringContent GetRequestContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }
    }
}
