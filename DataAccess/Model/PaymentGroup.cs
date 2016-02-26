using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Model
{

    /// <summary>
    /// A group of payments which belong together (e.g. one person paid for a cinema for four others).
    /// </summary>
    public class PaymentGroup
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public int GroupId { get; set; }

        public virtual Group Group { get; set; }



        public virtual ICollection<Payment> Payments { get; private set; }


        public PaymentGroup()
        {
            Payments = new List<Payment>();
        }
    }
}
