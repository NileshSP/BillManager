using BillManagerApi;
using BillManagerTests.Integration.HostConfiguration;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net.Http;

namespace BillManagerTests.Integration
{
    public class BaseIntegrationTest
    {
        protected CustomWebApplicationFactory<Startup> _factory;
        protected HttpClient _client;

        [OneTimeSetUp]
        public void SetUp()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
