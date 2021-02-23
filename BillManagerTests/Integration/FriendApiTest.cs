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
    public class FriendApiTest : BaseIntegrationTest
    {
        private readonly string _friendUri = "api/Friend";
        private string GetFriendUri(string appendPath)
        {
            return $"{_friendUri}/{appendPath}";
        }

        /// <summary>
        /// Performs checks of all Friend-related API methods one by one with checks of sanity in between
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task IntegrationAllFriendMethods()
        {
            FriendModel newFriend = new FriendModel
            {
                FirstName = "Integration Test FirstName",
                LastName = "Integration Test LastName"
            };

            List<FriendModel> newFriends = new List<FriendModel>
            {
                new FriendModel { FirstName = "Integration Test FirstName 1", LastName = "Integration Test LastName 1" },
                new FriendModel { FirstName = "Integration Test FirstName 2", LastName = "Integration Test LastName 2" },
                new FriendModel { FirstName = "Integration Test FirstName 3", LastName = "Integration Test LastName 3" },
            };

            // New Friend creation test
            int newFriendId = await CreateNewFriend(_client, newFriend);

            // Load newly created Friend test
            FriendModel FriendLoaded = await GetFriend(_client, newFriendId, newFriend);

            // List Friends and check that newly created Friend is there
            await GetFriendLists(_client, newFriendId, newFriend);

            // Modifying Friend and saving it
            await EditFriend(FriendLoaded, _client);

            // Loading the Friend again to ensure modifications saved
            await GetModifiedFriend(_client, newFriendId, FriendLoaded);

            // Deleting the Friend we just created
            await DeleteFriend(_client, newFriendId);

            // List all Friends again to ensure that Friend is deleted
            await EnsureFriendIsDeleted(_client, newFriendId);

            // New Friends creation test
            await CreateNewFriends(_client, newFriends);

            // List Friends and check that newly created Friends are there
            List<FriendModel> newFriendsList = await GetFriendsLists(_client, newFriends);

            // Modifying Friends and saving it
            List<FriendModel> modifiedFriends = await EditFriends(newFriendsList, _client);

            // Loading the Friend again to ensure modifications saved
            await GetModifiedFriends(_client, modifiedFriends);

            // Deleting the Friends we just created
            await DeleteFriends(_client, modifiedFriends);
        }

        private async Task DeleteFriends(HttpClient httpClient, List<FriendModel> newFriends)
        {
            HttpResponseMessage deleteResult = await httpClient.PostAsync(GetFriendUri("DeleteFriends"), Utilities.GetRequestContent(newFriends.Select(s => s.FriendId).ToList()));
            Assert.AreEqual(HttpStatusCode.OK, deleteResult.StatusCode, "DELETE API(DeleteFriend) method failed");
        }
        private async Task GetModifiedFriends(HttpClient httpClient, List<FriendModel> friendsLoaded)
        {
            HttpResponseMessage getResult = await httpClient.GetAsync(_friendUri);
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode, "GET (GetModifiedFriends) API method failed");

            List<FriendModel> modifiedFriendLoaded = JsonConvert.DeserializeObject<List<FriendModel>>(await getResult.Content.ReadAsStringAsync());
            Assert.IsNotNull(friendsLoaded);
            friendsLoaded.ForEach(friend =>
            {
                Assert.IsTrue(modifiedFriendLoaded.Any(p => p.FirstName == friend.FirstName), "GET (GetModifiedFriends) API method for friends modified check failed");
            });
        }

        private async Task<List<FriendModel>> EditFriends(List<FriendModel> friendLoaded, HttpClient httpClient)
        {
            List<FriendModel> listFriends = new List<FriendModel>();
            friendLoaded.ForEach(friend =>
            {
                listFriends.Add(new FriendModel { FriendId = friend.FriendId, FirstName = friend.FirstName + " Modified", LastName = friend.LastName + " Modified", Bills = null });
            });
            HttpResponseMessage putResult = await httpClient.PutAsync(GetFriendUri("UpdateFriends"), Utilities.GetRequestContent(listFriends));
            Assert.AreEqual(HttpStatusCode.OK, putResult.StatusCode, "PUT (EditFriends) API method failed");
            return listFriends;
        }

        public async Task<List<FriendModel>> GetFriendsLists(HttpClient httpClient, List<FriendModel> newFriends)
        {
            HttpResponseMessage listResult = await httpClient.GetAsync(_friendUri);
            Assert.AreEqual(HttpStatusCode.OK, listResult.StatusCode, "GET (GetFriendsLists) API method failed");

            List<FriendModel> listItems = JsonConvert.DeserializeObject<List<FriendModel>>(await listResult.Content.ReadAsStringAsync());
            List<FriendModel> listAdded = new List<FriendModel>();
            Assert.IsNotNull(listItems);
            if (newFriends != null)
            {
                newFriends.ForEach(friend =>
                {
                    Assert.IsTrue(listItems.Any(p => p.FirstName == friend.FirstName), "GET (GetFriendsLists) API method for friends added check failed");
                    listAdded.Add(listItems.FirstOrDefault(p => p.FirstName == friend.FirstName));
                });
            }
            else
            {
                listAdded.AddRange(listItems);
            }
            return listAdded;
        }

        private async Task CreateNewFriends(HttpClient httpClient, List<FriendModel> newFriends)
        {
            HttpResponseMessage FriendResult = await httpClient.PostAsync(GetFriendUri("AddFriends"), Utilities.GetRequestContent(newFriends));
            FriendResult.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, FriendResult.StatusCode, "Friend (CreateNewFriends) API method failed");
        }

        private async Task EnsureFriendIsDeleted(HttpClient httpClient, int newFriendId)
        {
            HttpResponseMessage listResult = await httpClient.GetAsync(_friendUri);
            Assert.AreEqual(HttpStatusCode.OK, listResult.StatusCode, "GET EnsureFriendIsDeleted API method failed");
            List<FriendModel> listItems = JsonConvert.DeserializeObject<List<FriendModel>>(listResult.Content.ReadAsStringAsync().Result);
            Assert.IsFalse(listItems.Any(p => p.FriendId == newFriendId));
        }

        private async Task DeleteFriend(HttpClient httpClient, int newFriendId)
        {
            HttpResponseMessage deleteResult = await httpClient.DeleteAsync(GetFriendUri(newFriendId.ToString()));
            Assert.AreEqual(HttpStatusCode.OK, deleteResult.StatusCode, "DELETE API(DeleteFriend) method failed");
        }

        private async Task GetModifiedFriend(HttpClient httpClient, int newFriendId, FriendModel FriendLoaded)
        {
            HttpResponseMessage getResult = await httpClient.GetAsync(GetFriendUri(newFriendId.ToString()));
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode, "GET (GetModifiedFriend) API method failed");

            FriendModel modifiedFriendLoaded = JsonConvert.DeserializeObject<FriendModel>(await getResult.Content.ReadAsStringAsync());
            Assert.AreEqual(FriendLoaded.FirstName, modifiedFriendLoaded.FirstName);
            Assert.AreEqual(FriendLoaded.LastName, modifiedFriendLoaded.LastName);
        }

        private async Task EditFriend(FriendModel FriendLoaded, HttpClient httpClient)
        {
            FriendLoaded.FirstName += " Modified";
            FriendLoaded.LastName += " Modified";
            HttpResponseMessage putResult = await httpClient.PutAsync(_friendUri, Utilities.GetRequestContent(FriendLoaded));
            Assert.AreEqual(HttpStatusCode.OK, putResult.StatusCode, "PUT (EditFriend) API method failed");
        }

        private async Task GetFriendLists(HttpClient httpClient, int newFriendId, FriendModel newFriend)
        {
            HttpResponseMessage listResult = await httpClient.GetAsync(_friendUri);
            Assert.AreEqual(HttpStatusCode.OK, listResult.StatusCode, "GET (GetFriendLists) API method failed");

            List<FriendModel> listItems = JsonConvert.DeserializeObject<List<FriendModel>>(await listResult.Content.ReadAsStringAsync());
            FriendModel FriendListItemLoaded = listItems.FirstOrDefault(p => p.FriendId == newFriendId);
            Assert.IsNotNull(FriendListItemLoaded);
            Assert.AreEqual(newFriendId, FriendListItemLoaded.FriendId);
            Assert.AreEqual(newFriend.FirstName, FriendListItemLoaded.FirstName);
        }

        public async Task<FriendModel> GetFriend(HttpClient httpClient, int newFriendId, FriendModel newFriend)
        {
            HttpResponseMessage getResult = await httpClient.GetAsync(GetFriendUri(newFriendId.ToString()));
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode, "GET/id (GetFriend) API method failed");

            FriendModel FriendLoaded = JsonConvert.DeserializeObject<FriendModel>(await getResult.Content.ReadAsStringAsync());
            Assert.AreEqual(newFriendId, FriendLoaded.FriendId);
            if (newFriend != null)
            {
                Assert.AreEqual(newFriend.FirstName, FriendLoaded.FirstName);
                Assert.AreEqual(newFriend.LastName, FriendLoaded.LastName);
            }
            return FriendLoaded;
        }

        public async Task<int> CreateNewFriend(HttpClient httpClient, FriendModel newFriend)
        {
            HttpResponseMessage FriendResult = await httpClient.PostAsync(_friendUri, Utilities.GetRequestContent(newFriend));
            FriendResult.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, FriendResult.StatusCode, "Friend (CreateNewFriend) API method failed");

            string stringResponse = await FriendResult.Content.ReadAsStringAsync();
            FriendModel friend = JsonConvert.DeserializeObject<FriendModel>(stringResponse);

            int newFriendId = int.Parse(friend.FriendId.ToString());
            Assert.IsTrue(friend.FriendId > 0);

            return newFriendId;
        }
    }
}
