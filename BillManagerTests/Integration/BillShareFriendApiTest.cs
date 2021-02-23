using BillManagerApi.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BillManagerTests.Integration
{
    public class BillShareFriendApiTest : BaseIntegrationTest
    {
        private readonly string _billUri = "api/Bill";
        private readonly string _friendUri = "api/Friend";
        private string getBillUri(string appendPath)
        {
            return $"{_billUri}/{appendPath}";
        }

        private string getFriendUri(string appendPath)
        {
            return $"{_friendUri}/{appendPath}";
        }

        private FriendApiTest _friendApi;
        private BillApiTest _billApi;

        /// <summary>
        /// Performs checks of all bill linked to friends and vice versa - related API methods one by one with checks of sanity in between
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task IntegrationAllBillShareFriendMethods()
        {
            //create a new friend
            _friendApi = new FriendApiTest();
            int friendId = await _friendApi.CreateNewFriend(_client, new FriendModel { FirstName = "BillShareFriend Friend FirstName", LastName = "BillShareFriend Friend LastName" });

            //create a new bill
            _billApi = new BillApiTest();
            int billId = await _billApi.CreateNewBill(_client, new BillModel { ExpenseDescription = "BillShareFriend Bill Expense", Amount = 500 });


            //From BillController Links/Unlink -- Start
            //link newly created friend with bill
            await LinkBillToFriend(_client, billId, friendId);

            //link newly created friend with bill
            await UnLinkBillToFriend(_client, billId, friendId);

            // Confirm bills has friends listed in output with share of each
            await CheckBillListHasFriendList(_client);
            //From BillController -- End


            //From FriendController for Links/Unlink -- Start
            //link newly created friend with bill
            await LinkFriendToBill(_client, billId, friendId);

            //link newly created friend with bill
            await UnLinkFriendToBill(_client, billId, friendId);

            // Confirm friends has bills listed in output
            await CheckFriendListHasBillList(_client);
            //From FriendController -- End

        }

        private async Task CheckFriendListHasBillList(HttpClient httpClient)
        {
            List<FriendModel> friendList = await _friendApi.GetFriendsLists(httpClient, null);
            Assert.IsNotNull(friendList);
            Assert.IsTrue(friendList.SelectMany(b => b.Bills).Count() > 0, "Friends does not have bills listed");
        }

        private async Task CheckBillListHasFriendList(HttpClient httpClient)
        {
            List<BillModel> billList = await _billApi.GetBillsLists(httpClient, null);
            Assert.IsNotNull(billList);
            Assert.IsTrue(billList.SelectMany(b => b.Friends).Count() > 0, "Bills does not have friends listed");
        }

        private async Task LinkBillToFriend(HttpClient httpClient, int billId, int friendId)
        {
            HttpResponseMessage getResult = await httpClient.GetAsync(getBillUri($"LinkItem/{billId}/{friendId}"));
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode, "LinkBillToFriend API method failed");

            //get friend
            FriendModel friend = await _friendApi.GetFriend(httpClient, friendId, null);

            //get bill
            BillModel bill = await _billApi.GetBill(httpClient, billId, null);

            Assert.IsTrue(friend.Bills.Contains(bill.ExpenseDescription), "Friend is not linked to Bill");

            Assert.IsTrue(bill.Friends.Any(f => f.FriendId == friend.FriendId), "Bill is not linked to Friend");
        }

        private async Task UnLinkBillToFriend(HttpClient httpClient, int billId, int friendId)
        {
            HttpResponseMessage getResult = await httpClient.GetAsync(getBillUri($"UnLinkItem/{billId}/{friendId}"));
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode, "UnLinkBillToFriend API method failed");

            //get bill
            BillModel modifiedLinkedBill = await _billApi.GetBill(httpClient, billId, null);

            Assert.IsTrue(!modifiedLinkedBill.Friends.Any(f => f.FriendId == friendId), "Bill still has friend linked");
        }

        private async Task LinkFriendToBill(HttpClient httpClient, int billId, int friendId)
        {
            HttpResponseMessage getResult = await httpClient.GetAsync(getFriendUri($"LinkItem/{friendId}/{billId}"));
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode, "LinkFriendToBill API method failed");

            //get friend
            FriendModel updatedfriend = await _friendApi.GetFriend(httpClient, friendId, null);

            //get bill
            BillModel updatedbill = await _billApi.GetBill(httpClient, billId, null);

            Assert.IsTrue(updatedfriend.Bills.Contains(updatedbill.ExpenseDescription), "Friend is not linked to Bill");

            Assert.IsTrue(updatedbill.Friends.Any(f => f.FriendId == updatedfriend.FriendId), "Bill is not linked to Friend");
        }

        private async Task UnLinkFriendToBill(HttpClient httpClient, int billId, int friendId)
        {
            HttpResponseMessage getResult = await httpClient.GetAsync(getFriendUri($"UnLinkItem/{friendId}/{billId}"));
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode, "UnLinkFriendToBill API method failed");

            //get Friend
            FriendModel modifiedLinkedFriend = await _friendApi.GetFriend(httpClient, billId, null);

            //get bill
            BillModel modifiedLinkedBill = await _billApi.GetBill(httpClient, billId, null);

            Assert.IsTrue(!modifiedLinkedFriend.Bills.Contains(modifiedLinkedBill.ExpenseDescription), "Bill still has friend linked");
        }
    }
}

