using System.ComponentModel.DataAnnotations;

namespace BillManagerApi.Models
{
    public class FriendShareBillModel
    {
        [Required]
        public int FriendId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public float AmountShare { get; set; }

    }
}
