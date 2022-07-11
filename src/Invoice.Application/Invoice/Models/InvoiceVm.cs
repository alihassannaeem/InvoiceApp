using Invoice.Application.Common.Mappings;
using Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Invoice.Application.Invoice.Models
{
    public class InvoiceVm : IMapFrom<Invoices>
    {
        public int Id { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string InvoiceNote { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public List<InvoiceItems> items { get; set; }
    }
}
