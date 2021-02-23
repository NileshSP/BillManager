using BillManagerApi.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BillManagerApi.Repositories.Entities
{
    public class Bill : IEntity
    {
        [Key]
        public int BillId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModified { get; set; }
        public string CreatedById { get; set; }
        [Required]
        public string ExpenseDescription { get; set; }
        [Required]
        public float Amount { get; set; }
        public virtual IList<BillShareFriend> BillShareFriends { get; set; }
    }
}
