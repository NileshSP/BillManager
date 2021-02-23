using BillManagerApi;
using BillManagerTests.Integration.HostConfiguration;
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
            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
