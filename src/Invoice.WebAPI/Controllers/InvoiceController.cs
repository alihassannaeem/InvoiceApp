using Invoice.Application.Auth.Login;
using Invoice.Application.Invoice.Command.CreateUpdateInvoice;
using Invoice.Application.Invoice.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invoice.WebAPI.Controllers
{
    [Authorize]
    public class InvoiceController : ApiController
    {
        [HttpPost("AddUpdateInvoice")]
        public async Task<IActionResult> AddUpdateInvoice([FromBody] CreateUpdateInvoiceCommand request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await Mediator.Send(new GetInvoicesQuery { });            
            return Ok(data);
        }
    }
}
