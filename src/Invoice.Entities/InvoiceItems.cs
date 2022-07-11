using System;
using System.ComponentModel.DataAnnotations;

namespace Invoice.Entities
{
    public class InvoiceItems
    {
        [Key]
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public decimal ItemRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}
