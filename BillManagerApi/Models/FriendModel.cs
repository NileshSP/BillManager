using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BillManagerApi.Models
{
    public class FriendModel
    {
        public int FriendId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public List<string> Bills { get; set; }
    }
}
