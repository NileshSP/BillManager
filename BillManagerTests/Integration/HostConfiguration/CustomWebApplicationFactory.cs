using BillManagerApi;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BillManagerTests.Integration.HostConfiguration
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
    }
}
