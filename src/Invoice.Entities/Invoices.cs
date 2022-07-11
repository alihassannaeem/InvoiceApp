using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Invoice.Entities
{
    public class Invoices
    {                
        [Key]
        public int Id { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string InvoiceNote { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }           
    }
}
