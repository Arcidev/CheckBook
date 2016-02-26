using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CheckBook.DataAccess.Model
{

    /// <summary>
    /// A group of users which share their payments.
    /// </summary>
    public class Group
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<PaymentGroup> PaymentGroups { get; private set; }


        public Group()
        {
            PaymentGroups = new List<PaymentGroup>();
        }
    }
}
