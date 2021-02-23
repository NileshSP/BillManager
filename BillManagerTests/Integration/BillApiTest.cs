using BillManagerApi.Models;
using BillManagerTests.Shared;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BillManagerTests.Integration
{
    public class BillApiTest : BaseIntegrationTest
    {
        private readonly string _billUri = "api/Bill";
        private string GetBillUri(string appendPath)
        {
            return $"{_billUri}/{appendPath}";
        }

        /// <summary>
        /// Performs checks of all Bill-related API methods one by one with checks of sanity in between
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task IntegrationAllBillMethods()
        {
            BillModel newBill = new BillModel
            {
                ExpenseDescription = "Integration Test Expense",
                Amount = 100
            };

            List<BillModel> newBills = new List<BillModel>
            {
                new BillModel { ExpenseDescription = "Integration Test ExpenseDescription 1", Amount = 100 },
                new BillModel { ExpenseDescription = "Integration Test ExpenseDescription 2", Amount = 200 },
                new BillModel { ExpenseDescription = "Integration Test ExpenseDescription 3", Amount = 300 },
            };

            // New Bill creation test
            int newBillId = await CreateNewBill(_client, newBill);

            // Load newly created Bill test
            BillModel billLoaded = await GetBill(_client, newBillId, newBill);

            // List Bills and check that newly created Bill is there
            await GetBillLists(_client, newBillId, newBill);

            // Modifying Bill and saving it
            await EditBill(billLoaded, _client);

            // Loading the Bill again to ensure modifications saved
            await GetModifiedBill(_client, newBillId, billLoaded);

            // Deleting the Bill we just created
            await DeleteBill(_client, newBillId);

            // List all Bills again to ensure that Bill is deleted
            await EnsureBillIsDeleted(_client, newBillId);

            // New Bills creation test
            await CreateNewBills(_client, newBills);

            // List Bills and check that newly created Bills are there
            List<BillModel> newBillsList = await GetBillsLists(_client, newBills);

            // Modifying Bills and saving it
            List<BillModel> modifiedBills = await EditBills(newBillsList, _client);

            // Loading the Bill again to ensure modifications saved
            await GetModifiedBills(_client, modifiedBills);

            // Deleting the Bills we just created
            await DeleteBills(_client, modifiedBills);
        }

        private async Task DeleteBills(HttpClient httpClient, List<BillModel> newBills)
        {
            HttpResponseMessage deleteResult = await httpClient.PostAsync(GetBillUri("DeleteBills"), Utilities.GetRequestContent(newBills.Select(s => s.BillId).ToList()));
            Assert.AreEqual(HttpStatusCode.OK, deleteResult.StatusCode, "DELETE API(DeleteBills) method failed");
        }
        private async Task GetModifiedBills(HttpClient httpClient, List<BillModel> billsLoaded)
        {
            HttpResponseMessage getResult = await httpClient.GetAsync(_billUri);
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode, "GET (GetModifiedBills) API method failed");

            List<BillModel> modifiedbillLoaded = JsonConvert.DeserializeObject<List<BillModel>>(await getResult.Content.ReadAsStringAsync());
            Assert.IsNotNull(billsLoaded);
            billsLoaded.ForEach(Bill =>
            {
                Assert.IsTrue(modifiedbillLoaded.Any(p => p.ExpenseDescription == Bill.ExpenseDescription), "GET (GetModifiedBills) API method for Bills modified check failed");
            });
        }

        private async Task<List<BillModel>> EditBills(List<BillModel> billLoaded, HttpClient httpClient)
        {
            List<BillModel> listBills = new List<BillModel>();
            billLoaded.ForEach(Bill =>
            {
                listBills.Add(new BillModel { BillId = Bill.BillId, ExpenseDescription = Bill.ExpenseDescription + " Modified", Amount = Bill.Amount + 5, Friends = null });
            });
            HttpResponseMessage putResult = await httpClient.PutAsync(GetBillUri("UpdateBills"), Utilities.GetRequestContent(listBills));
            Assert.AreEqual(HttpStatusCode.OK, putResult.StatusCode, "PUT (EditBills) API method failed");
            return listBills;
        }

        public async Task<List<BillModel>> GetBillsLists(HttpClient httpClient, List<BillModel> newBills)
        {
            HttpResponseMessage listResult = await httpClient.GetAsync(_billUri);
            Assert.AreEqual(HttpStatusCode.OK, listResult.StatusCode, "GET (GetBillsLists) API method failed");

            List<BillModel> listItems = JsonConvert.DeserializeObject<List<BillModel>>(await listResult.Content.ReadAsStringAsync());
            List<BillModel> listAdded = new List<BillModel>();
            Assert.IsNotNull(listItems);
            if (newBills != null)
            {
                newBills.ForEach(Bill =>
                {
                    Assert.IsTrue(listItems.Any(p => p.ExpenseDescription == Bill.ExpenseDescription), "GET (GetBillsLists) API method for Bills added check failed");
                    listAdded.Add(listItems.FirstOrDefault(p => p.ExpenseDescription == Bill.ExpenseDescription));
                });
            }
            else
            {
                listAdded.AddRange(listItems);
            }
            return listAdded;
        }

        private async Task CreateNewBills(HttpClient httpClient, List<BillModel> newBills)
        {
            HttpResponseMessage BillResult = await httpClient.PostAsync(GetBillUri("AddBills"), Utilities.GetRequestContent(newBills));
            BillResult.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, BillResult.StatusCode, "Bill (CreateNewBills) API method failed");
        }

        private async Task EnsureBillIsDeleted(HttpClient httpClient, int newBillId)
        {
            HttpResponseMessage listResult = await httpClient.GetAsync(_billUri);
            Assert.AreEqual(HttpStatusCode.OK, listResult.StatusCode, "GET EnsureBillIsDeleted API method failed");
            List<BillModel> listItems = JsonConvert.DeserializeObject<List<BillModel>>(listResult.Content.ReadAsStringAsync().Result);
            Assert.IsFalse(listItems.Any(p => p.BillId == newBillId));
        }

        private async Task DeleteBill(HttpClient httpClient, int newBillId)
        {
            HttpResponseMessage deleteResult = await httpClient.DeleteAsync(GetBillUri(newBillId.ToString()));
            Assert.AreEqual(HttpStatusCode.OK, deleteResult.StatusCode, "DELETE API(DeleteBill) method failed");
        }

        private async Task GetModifiedBill(HttpClient httpClient, int newBillId, BillModel billLoaded)
        {
            HttpResponseMessage getResult = await httpClient.GetAsync(GetBillUri(newBillId.ToString()));
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode, "GET (GetModifiedBill) API method failed");

            BillModel modifiedbillLoaded = JsonConvert.DeserializeObject<BillModel>(await getResult.Content.ReadAsStringAsync());
            Assert.AreEqual(billLoaded.ExpenseDescription, modifiedbillLoaded.ExpenseDescription);
            Assert.AreEqual(billLoaded.Amount, modifiedbillLoaded.Amount);
        }

        private async Task EditBill(BillModel billLoaded, HttpClient httpClient)
        {
            billLoaded.ExpenseDescription += " Modified";
            billLoaded.Amount += 5;
            HttpResponseMessage putResult = await httpClient.PutAsync(_billUri, Utilities.GetRequestContent(billLoaded));
            Assert.AreEqual(HttpStatusCode.OK, putResult.StatusCode, "PUT (EditBill) API method failed");
        }

        private async Task GetBillLists(HttpClient httpClient, int newBillId, BillModel newBill)
        {
            HttpResponseMessage listResult = await httpClient.GetAsync(_billUri);
            Assert.AreEqual(HttpStatusCode.OK, listResult.StatusCode, "GET (GetBillLists) API method failed");

            List<BillModel> listItems = JsonConvert.DeserializeObject<List<BillModel>>(await listResult.Content.ReadAsStringAsync());
            BillModel BillListItemLoaded = listItems.FirstOrDefault(p => p.BillId == newBillId);
            Assert.IsNotNull(BillListItemLoaded);
            Assert.AreEqual(newBillId, BillListItemLoaded.BillId);
            Assert.AreEqual(newBill.ExpenseDescription, BillListItemLoaded.ExpenseDescription);
        }

        public async Task<BillModel> GetBill(HttpClient httpClient, int newBillId, BillModel newBill)
        {
            HttpResponseMessage getResult = await httpClient.GetAsync(GetBillUri(newBillId.ToString()));
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode, "GET/id (GetBill) API method failed");

            BillModel billLoaded = JsonConvert.DeserializeObject<BillModel>(await getResult.Content.ReadAsStringAsync());
            Assert.AreEqual(newBillId, billLoaded.BillId);
            if (newBill != null)
            {
                Assert.AreEqual(newBill.ExpenseDescription, billLoaded.ExpenseDescription);
                Assert.AreEqual(newBill.Amount, billLoaded.Amount);
            }
            return billLoaded;
        }

        public async Task<int> CreateNewBill(HttpClient httpClient, BillModel newBill)
        {
            HttpResponseMessage BillResult = await httpClient.PostAsync(_billUri, Utilities.GetRequestContent(newBill));
            BillResult.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, BillResult.StatusCode, "Bill (CreateNewBill) API method failed");

            string stringResponse = await BillResult.Content.ReadAsStringAsync();
            BillModel Bill = JsonConvert.DeserializeObject<BillModel>(stringResponse);

            int newBillId = int.Parse(Bill.BillId.ToString());
            Assert.IsTrue(Bill.BillId > 0);

            return newBillId;
        }
    }
}
