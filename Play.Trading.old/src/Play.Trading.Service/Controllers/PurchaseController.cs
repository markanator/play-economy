using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Trading.Service.Contracts;
using Play.Trading.Service.Dtos;

namespace Play.Trading.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // so we know which user is calling this API
    public class PurchaseController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        public PurchaseController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(SubmitPurchaseDto purchase)
        {
            var userId = User.FindFirstValue("sub");
            var correlationId = Guid.NewGuid();

            var message = new PurchaseRequested(
                Guid.Parse(userId),
                purchase.ItemId.Value,
                purchase.Quantity,
                correlationId
            );
            // pub msg
            await _publishEndpoint.Publish(message);
            // return 202, no need to make the end-user to wait for process to end
            return Accepted();
        }
    }
}