using Invoice.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Invoice.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Invoices> Invoices { get; set; }
        DbSet<InvoiceItems> InvoiceItems { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
