using BillManagerApi.Models;
using BillManagerApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillManagerApi.Controllers
{
    [Route("api/[controller]")]
    public class FriendController : Controller
    {
        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        // GET: api/Friend
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return ((new OkObjectResult(await _friendService.GetList())) as IActionResult);
        }

        // GET: api/Friend/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return ((new OkObjectResult(await _friendService.GetItem(id))) as IActionResult);
        }

        // PUT: api/Friend/
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]FriendModel friend)
        {
            return _friendService.GetIActionResult(await _friendService.PutItem(friend));
        }

        // POST: api/Friend
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]FriendModel friend)
        {
            int? newId = await _friendService.PostItem(friend);
            return (new OkObjectResult(new { FriendId = newId }) as IActionResult);
        }

        // DELETE: api/Friend/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return _friendService.GetIActionResult(await _friendService.DeleteItem(id));
        }

        // PUT: api/Friend/UpdateFriends
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateFriends([FromBody]List<FriendModel> friends)
        {
            return _friendService.GetIActionResult(await _friendService.PutItems(friends));
        }

        // POST: api/Friend/AddFriends
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddFriends([FromBody]List<FriendModel> friends)
        {
            return _friendService.GetIActionResult(await _friendService.PostItems(friends));
        }

        // DELETE: api/Friend/DeleteFriends
        [HttpDelete, HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteFriends([FromBody]List<int> ids)
        {
            return _friendService.GetIActionResult(await _friendService.DeleteItems(ids));
        }

        // Add Link: api/Friend/LinkItem/{FriendId}/{BillId}
        [HttpPost]
        [Route("LinkItem/{searchId}/{linkId}")]
        public async Task<IActionResult> LinkItem(int searchId, int linkId)
        {
            return _friendService.GetIActionResult(await _friendService.LinkItem(searchId, linkId));
        }

        // Remove Link: api/Friend/UnLinkItem/{FriendId}/{BillId}
        [HttpPost]
        [Route("UnLinkItem/{searchId}/{linkId}")]
        public async Task<IActionResult> UnLinkItem(int searchId, int linkId)
        {
            return _friendService.GetIActionResult(await _friendService.UnLinkItem(searchId, linkId));
        }
    }
}
