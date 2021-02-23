using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillManagerApi.Services.Interfaces
{
    public interface ICommonService<T>
    {
        Task<IEnumerable<T>> GetList();
        Task<T> GetItem(int id);
        Task<int?> PostItem(T item);
        Task<bool> PutItem(T item);
        Task<bool> DeleteItem(int id);
        Task<bool> PostItems(IEnumerable<T> items);
        Task<bool> PutItems(IEnumerable<T> items);
        Task<bool> DeleteItems(IEnumerable<int> idList);
        Task<bool> LinkItem(int searchId, int linkId);
        Task<bool> UnLinkItem(int searchId, int linkId);
        IActionResult GetIActionResult(bool result);
    }
}
