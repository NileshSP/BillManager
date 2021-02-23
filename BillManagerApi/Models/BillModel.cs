using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BillManagerApi.Models
{
    public class BillModel
    {
        public int BillId { get; set; }
        [Required]
        public string ExpenseDescription { get; set; }
        [Required]
        public float Amount { get; set; }
        public ICollection<FriendShareBillModel> Friends { get; set; }
    }
}
