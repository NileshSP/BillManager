using BillManagerApi.Controllers;
using BillManagerApi.Repositories;
using BillManagerApi.Repositories.Interfaces;
using BillManagerApi.Services;
using BillManagerApi.Services.Interfaces;
using BillManagerTests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BillManagerTests.Controller
{
    public class BaseControllerTest
    {
        protected FriendController _friendController;
        protected BillController _billController;
        protected IDBContext _testContext;

        [OneTimeSetUp]
        public void Setup()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<BillManagerDBContext>(options => options.UseInMemoryDatabase("ControllerBillManagerDB"));
            services.AddTransient<IDBContext, BillManagerDBContext>();
            services.AddTransient<IFriendService, FriendService>();
            services.AddTransient<FriendController>();
            services.AddTransient<IBillService, BillService>();
            services.AddTransient<BillController>();
            services.AddLogging();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            _testContext = serviceProvider.GetService<IDBContext>();
            _testContext.SeedTestData().Wait();
            _friendController = serviceProvider.GetService<FriendController>();
            _billController = serviceProvider.GetService<BillController>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _friendController.Dispose();
            _billController.Dispose();
            _testContext.Dispose();
        }
    }
}
