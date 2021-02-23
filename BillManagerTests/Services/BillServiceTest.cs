using BillManagerApi.Models;
using BillManagerApi.Repositories.Entities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillManagerTests.Services
{
    public class BillServiceTest : BaseServiceTest
    {
        /// <summary>
        /// Performs checks of all Bill-related service methods with checks of sanity in between
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task BillServiceMethods()
        {
            // Check for appropriate billshare for each friend of a bill
            await AppropriateBillShareInBill();
        }

        private async Task AppropriateBillShareInBill()
        {
            Bill bill = _testContext.Bill.Where(b => b.BillShareFriends.Count > 1).FirstOrDefault();
            Assert.IsNotNull(bill);
            List<FriendShareBillModel> friendsShare = await _billService.GetFriendShareBills(bill);
            float billShareAmount = bill.Amount / friendsShare.Count;
            friendsShare.ForEach(friend =>
            {
                Assert.IsTrue(friend.AmountShare == billShareAmount, "Bill share is not appropriate");
            });
        }
    }
}
