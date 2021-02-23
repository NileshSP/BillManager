using BillManagerApi.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BillManagerApi.Repositories.Entities
{

    public class Friend : IEntity
    {
        [Key]
        public int FriendId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModified { get; set; }
        public string CreatedById { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public virtual IList<BillShareFriend> BillShareFriends { get; set; }
    }
}
