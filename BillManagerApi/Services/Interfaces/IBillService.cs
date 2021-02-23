using BillManagerApi.Models;
using BillManagerApi.Repositories.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillManagerApi.Services.Interfaces
{
    public interface IBillService : ICommonService<BillModel>
    {
        Task<List<FriendShareBillModel>> GetFriendShareBills(Bill bill);
    }
}
