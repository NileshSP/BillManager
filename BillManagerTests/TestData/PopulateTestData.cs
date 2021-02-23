using BillManagerApi.Repositories;
using BillManagerApi.Repositories.Interfaces;
using System.Threading.Tasks;

namespace BillManagerTests.TestData
{
    internal static class PopulateTestData
    {
        public static Task SeedTestData(this IDBContext context)
        {
            return context.SeedSampleData();
        }
    }
}
