using BillManagerApi.Models;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BillManagerTests.Controller
{
    public class FriendControllerTest : BaseControllerTest
    {
        [Order(1)]
        [Test]
        public void GetFriendList()
        {
            IActionResult actionResult = _friendController.Get().Result;
            OkObjectResult result = actionResult as OkObjectResult;
            Assert.True(result is OkObjectResult);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            IEnumerable<FriendModel> resultValue = result.Value as IEnumerable<FriendModel>;
            Assert.IsNotNull(resultValue);
            Assert.Greater(resultValue.Count(), 1);
        }

        [Order(2)]
        [Test]
        public void GetFriend()
        {
            IActionResult actionResult = _friendController.Get(1).Result;
            OkObjectResult result = actionResult as OkObjectResult;
            FriendModel resultValue = result.Value as FriendModel;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(resultValue);
        }

        [Order(3)]
        [Test]
        public void AddFriend()
        {
            IActionResult actionResult = _friendController.Post(new FriendModel { FirstName = "Test", LastName = "User", Bills = new List<string>() }).Result;
            OkObjectResult result = actionResult as OkObjectResult;
            Assert.IsNotNull(result);
        }

        [Order(4)]
        [Test]
        public void UpdateFriend()
        {
            IActionResult actionResult = _friendController.Put(new FriendModel { FriendId = 11, FirstName = "Tester 11", LastName = "User 11" }).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(5)]
        [Test]
        public void DeleteFriend()
        {
            IActionResult actionResult = _friendController.Delete(11).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(6)]
        [Test]
        public void AddFriends()
        {
            IActionResult actionResult = _friendController
                                    .AddFriends(
                                        Enumerable.Range(11, 3)
                                        .Select(i => new FriendModel { FirstName = $"Test {i}", LastName = $"User {i}" })
                                        .ToList<FriendModel>()
                                    ).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(7)]
        [Test]
        public void UpdateFriends()
        {
            IActionResult actionResult = _friendController
                                    .UpdateFriends(
                                        Enumerable.Range(11, 3)
                                        .Select(i => new FriendModel { FriendId = i, FirstName = $"Test {i} Modified", LastName = $"User {i} Modified" })
                                        .ToList<FriendModel>()
                                    ).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(8)]
        [Test]
        public void FriendLinkBill()
        {
            IActionResult actionResult = _friendController.LinkItem(12, 2).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(9)]
        [Test]
        public void FriendUnLinkBill()
        {
            IActionResult actionResult = _friendController.UnLinkItem(12, 2).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }

        [Order(10)]
        [Test]
        public void DeleteFriends()
        {
            IActionResult actionResult = _friendController.DeleteFriends(new List<int> { 11, 12, 13 }).Result;
            OkResult result = actionResult as OkResult;
            Assert.IsNotNull(result);
        }
    }
}