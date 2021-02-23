using BillManagerApi.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillManagerApi.Repositories.Interfaces
{
    public interface IDBContext
    {
        DbSet<Friend> Friend { get; set; }
        DbSet<Bill> Bill { get; set; }
        DbSet<BillShareFriend> BillShareFriend { get; set; }
        DbContext DatabaseContext { get; }

        void Dispose();
    }
}
