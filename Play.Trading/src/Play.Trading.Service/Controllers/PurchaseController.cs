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
using Play.Trading.Service.StateMachines;

namespace Play.Trading.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // so we know which user is calling this API
    public class PurchaseController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<GetPurchaseState> _requestClient;
        public PurchaseController(
            IPublishEndpoint publishEndpoint,
            IRequestClient<GetPurchaseState> requestClient
        )
        {
            _publishEndpoint = publishEndpoint;
            _requestClient = requestClient;
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

            await _publishEndpoint.Publish(message);

            return AcceptedAtAction(nameof(GetStatusAsync), new { correlationId }, new { correlationId });
        }

        [HttpGet("status/{correlationId}")]
        public async Task<IActionResult> GetStatusAsync(Guid correlationId)
        {
            // msg goes into overall queue
            var response = await _requestClient.GetResponse<PurchaseState>(new GetPurchaseState(correlationId));
            var purchaseState = response.Message;
            var purchase = new PurchaseDto(
                purchaseState.UserId,
                purchaseState.ItemId,
                purchaseState.PurchaseTotal,
                purchaseState.Quantity,
                purchaseState.CurrentState,
                purchaseState.ErrorMessage,
                purchaseState.Received,
                purchaseState.LastUpdated
            );
            return Ok(purchase);
        }
    }
}