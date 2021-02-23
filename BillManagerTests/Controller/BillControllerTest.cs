using BillManagerApi.Models;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BillManagerTests.Controller
{
    public class BillControllerTest : BaseControllerTest
    {

        [Order(1)]
        [Test]
        public void GetBillList()
        {
            IActionResult actionResult = _billController.Get().Result;
            OkObjectResult result = actionResult as OkObjectResult;
            Assert.True(result is OkObjectResult);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            IEnumerable<BillModel> resultValue = result.Value as IEnumerable<BillModel>;
            Assert.IsNotNull(resultValue);
            Assert.Greater(resultValue.Count(), 1);
        }

        [Order(2)]
        [Test]
        public void GetBill()
        {
            IActionResult actionResult = _billController.Get(1).Result;
            OkObjectResult result = actionResult as OkObjectResult;
            BillModel resultValue = result.Value as BillModel;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(resultValue);
        }

        [Order(3)]
        [Test]
        public void AddBill()
        {
            IActionResult actionResult = _billController.Post(new BillModel { ExpenseDescription = "Expense 6", Amount = 106 }).Result;
            OkObjectResult result = actionResult as OkObjectResult;
            Assert.IsNotNull(result);
        }

        [Order(4)]
        [Test]
        public void UpdateBill()
        {
            IActionResult actionResult = _billController.Put(new BillModel { BillId = 6, ExpenseDescription = "Expense 6", Amount = 116 }).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(5)]
        [Test]
        public void DeleteBill()
        {
            IActionResult actionResult = _billController.Delete(6).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(6)]
        [Test]
        public void AddBills()
        {
            IActionResult actionResult = _billController
                                    .AddBills(
                                        Enumerable.Range(6, 3)
                                        .Select(i => new BillModel { ExpenseDescription = $"Expense {i}", Amount = 200 + i })
                                        .ToList<BillModel>()
                                    ).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(7)]
        [Test]
        public void UpdateBills()
        {
            IActionResult actionResult = _billController
                                    .UpdateBills(
                                        Enumerable.Range(6, 3)
                                        .Select(i => new BillModel { BillId = i, ExpenseDescription = $"Expense {i} Modified", Amount = 300 + i })
                                        .ToList<BillModel>()
                                    ).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(8)]
        [Test]
        public void BillLinkBill()
        {
            IActionResult actionResult = _billController.LinkItem(1, 1).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(9)]
        [Test]
        public void BillUnLinkBill()
        {
            IActionResult actionResult = _billController.UnLinkItem(1, 1).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(10)]
        [Test]
        public void DeleteBills()
        {
            IActionResult actionResult = _billController.DeleteBills(new List<int> { 6, 7, 8 }).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }
    }
}