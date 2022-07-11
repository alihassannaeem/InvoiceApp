using Invoice.Application.Common.Interfaces;
using Invoice.Application.Common.Models;
using Invoice.Application.Invoice.Models;
using Invoice.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Invoice.Application.Invoice.Command.CreateUpdateInvoice
{
    public class CreateUpdateInvoiceCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string InvoiceNote { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; } = false;
        public List<InvoiceItemsVm> items { get; set; }

        public class CreateUpdateInvoiceCommandHandler : IRequestHandler<CreateUpdateInvoiceCommand, Result<bool>>
        {
            private readonly IApplicationDbContext _context;

            public CreateUpdateInvoiceCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Result<bool>> Handle(CreateUpdateInvoiceCommand request, CancellationToken cancellationToken)
            {
                Invoices invoice = new Invoices();                
                try
                {
                    if (request.Id == 0)
                    {
                        invoice.IsPaid = request.IsPaid;
                        invoice.PurchaseOrderNo = request.PurchaseOrderNo;
                        invoice.CreatedDate = DateTime.Now;
                        invoice.Amount = request.Amount;
                        invoice.InvoiceNote = request.InvoiceNote;
                        await _context.Invoices.AddAsync(invoice);

                        await _context.SaveChangesAsync();

                        foreach (var item in request.items)
                        {
                            await _context.InvoiceItems.AddAsync(new InvoiceItems
                            {
                                InvoiceId = invoice.Id,
                                ItemName = item.ItemName,
                                Qty = item.Qty,
                                ItemRate = item.ItemRate,
                                CreatedDate = DateTime.Now
                            });
                        }                        

                        await _context.SaveChangesAsync();

                        return Result<bool>.Success(true);
                    }
                    else
                    {
                        var record = await _context.Invoices.FirstOrDefaultAsync(_ => _.Id == request.Id);

                        if (record != null)
                        {
                            record.IsPaid = request.IsPaid;
                            record.PurchaseOrderNo = request.PurchaseOrderNo;
                            record.ModifiedDate = DateTime.Now;
                            record.Amount = request.Amount;
                            record.InvoiceNote = request.InvoiceNote;
                            _context.Invoices.Update(record);

                            await _context.SaveChangesAsync();

                            foreach (var item in request.items)
                            {
                                var invoiceItem = await _context.InvoiceItems.FirstOrDefaultAsync(_ => _.Id == item.Id);
                                if (invoiceItem != null)
                                {
                                    invoiceItem.ItemName = item.ItemName;
                                    invoiceItem.Qty = item.Qty;
                                    invoiceItem.ItemRate = item.ItemRate;
                                    invoiceItem.ModifiedDate = DateTime.Now;
                                    _context.InvoiceItems.Update(invoiceItem);
                                }
                                else
                                {
                                    await _context.InvoiceItems.AddAsync(new InvoiceItems
                                    {
                                        InvoiceId = invoice.Id,
                                        ItemName = item.ItemName,
                                        Qty = item.Qty,
                                        ItemRate = item.ItemRate,
                                        CreatedDate = DateTime.Now
                                    });
                                }
                            }

                            await _context.SaveChangesAsync();
                            
                            return Result<bool>.Success(true);
                        }

                        return Result<bool>.Failed(new string[] { $"record not found ({request.Id})" });
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add("Request", request);                    

                    return Result<bool>.Failed(new string[] { ex.Message });
                }
            }
        }
    }
}
