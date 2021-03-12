using BillManagerApi.Models;
using BillManagerApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : Controller
    {
        private readonly IBillService _billService;

        public BillController(IBillService BillService)
        {
            _billService = BillService;
        }

        // GET: api/Bills
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return ((new OkObjectResult(await _billService.GetList())) as IActionResult);
        }

        // GET: api/Bill/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return ((new OkObjectResult(await _billService.GetItem(id))) as IActionResult);
        }

        // PUT: api/Bill/
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]BillModel bill)
        {
            return _billService.GetIActionResult(await _billService.PutItem(bill));
        }

        // POST: api/Bill
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]BillModel bill)
        {
            int? newId = await _billService.PostItem(bill);
            return (new OkObjectResult(new { BillId = newId }) as IActionResult);
        }

        // DELETE: api/Bill/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return _billService.GetIActionResult(await _billService.DeleteItem(id));
        }

        // POST: api/Bill/AddBills
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddBills([FromBody]List<BillModel> bills)
        {
            return _billService.GetIActionResult(await _billService.PostItems(bills));
        }

        // PUT: api/Bill/UpdateBills
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateBills([FromBody]List<BillModel> bills)
        {
            return _billService.GetIActionResult(await _billService.PutItems(bills));
        }

        // DELETE: api/Bill/DeleteBills
        [HttpDelete, HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteBills([FromBody]List<int> ids)
        {
            return _billService.GetIActionResult(await _billService.DeleteItems(ids));
        }

        // Add Link: api/Bill/LinkItem/{BillId}/{FriendId}
        [HttpGet]
        [Route("LinkItem/{searchId}/{linkId}")]
        public async Task<IActionResult> LinkItem(int searchId, int linkId)
        {
            return _billService.GetIActionResult(await _billService.LinkItem(searchId, linkId));
        }

        // Remove Link: api/Bill/UnLinkItem/{BillId}/{FriendId}
        [HttpGet]
        [Route("UnLinkItem/{searchId}/{linkId}")]
        public async Task<IActionResult> UnLinkItem(int searchId, int linkId)
        {
            return _billService.GetIActionResult(await _billService.UnLinkItem(searchId, linkId));
        }
    }
}
