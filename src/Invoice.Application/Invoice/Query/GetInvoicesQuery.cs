using AutoMapper;
using AutoMapper.QueryableExtensions;
using Invoice.Application.Common.Interfaces;
using Invoice.Application.Common.Models;
using Invoice.Application.Invoice.Models;
using Invoice.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Invoice.Application.Invoice.Query
{
    public class GetInvoicesQuery : IRequest<List<InvoiceVm>>
    {

        public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, List<InvoiceVm>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;
            public GetInvoicesQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<InvoiceVm>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
            {
                var query = _context.Invoices.AsQueryable();

                var result = await query.ProjectTo<InvoiceVm>(_mapper.ConfigurationProvider).ToListAsync();

                result.Select(c =>
                {
                    c.items = _context.InvoiceItems.Where(i => i.InvoiceId == c.Id).ToList();
                    return c;
                }).ToList();

                return new List<InvoiceVm>(result);
            }
        }
    }
}
