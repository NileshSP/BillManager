
using BillManagerApi.Repositories;
using BillManagerApi.Repositories.Interfaces;
using BillManagerApi.Services;
using BillManagerApi.Services.Interfaces;
using BillManagerTests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BillManagerTests.Services
{
    public class BaseServiceTest
    {
        protected IFriendService _friendService;
        protected IBillService _billService;
        protected IDBContext _testContext;

        [OneTimeSetUp]
        public void Setup()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<BillManagerDBContext>(options => options.UseInMemoryDatabase("ServicesBillManagerDB"));
            services.AddTransient<IDBContext, BillManagerDBContext>();
            services.AddTransient<IFriendService, FriendService>();
            services.AddTransient<IBillService, BillService>();
            services.AddLogging();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            _testContext = serviceProvider.GetService<IDBContext>();
            _testContext.SeedTestData().Wait();
            _friendService = serviceProvider.GetService<IFriendService>();
            _billService = serviceProvider.GetService<IBillService>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _testContext.Dispose();
        }
    }
}

