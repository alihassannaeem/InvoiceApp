using System;
using System.Collections.Generic;
using System.Text;

namespace Invoice.Application.Invoice.Models
{
    public class InvoiceItemsVm
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public decimal ItemRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
