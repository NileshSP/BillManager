using System;

namespace BillManagerApi.Repositories.Interfaces
{
    public interface IEntity
    {
        DateTime? DateModified { get; set; }
        string CreatedById { get; set; }
    }
}
