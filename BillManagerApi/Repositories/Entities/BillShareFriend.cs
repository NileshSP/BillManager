using System.ComponentModel.DataAnnotations;

namespace BillManagerApi.Repositories.Entities
{
    public class BillShareFriend
    {
        [Required]
        public int BillId { get; set; }
        public Bill Bill { get; set; }
        [Required]
        public int FriendId { get; set; }
        public Friend Friend { get; set; }
    }
}
