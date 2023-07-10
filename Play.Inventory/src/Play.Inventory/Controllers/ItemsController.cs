using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Entities;
using static Play.Inventory.Dtos;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private const string AdminRole = "Admin";
        private readonly IRepository<InventoryItem> itemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;
        public ItemsController(IRepository<InventoryItem> _itemsRepository, IRepository<CatalogItem> _catalogItemsRepository)
        {
            this.itemsRepository = _itemsRepository;
            this.catalogItemsRepository = _catalogItemsRepository;
        }

        [HttpGet("{UserId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    Console.WriteLine("userId is empty");
                    return BadRequest();
                }
                // get userId from token
                var currentUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                // make sure the user is either an admin or the user is requesting their own inventory
                if (Guid.Parse(currentUserId) != userId && !User.IsInRole(AdminRole))
                {
                    // if not, return 403 Forbidden
                    return Forbid();
                }
                // all good, try to return the inventory
                var inventoryItemEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId);
                var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
                var catalogItemEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

                var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
                {
                    var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                    return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
                });
                if (inventoryItemDtos == null)
                {
                    Console.WriteLine("inventoryItemDtos is null");
                    throw new ArgumentNullException(nameof(inventoryItemDtos));
                }

                return Ok(inventoryItemDtos);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server Error");
            }
        }

        [HttpPost]
        [Authorize(Roles = AdminRole)]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await itemsRepository.GetAsync(
                item => item.UserId == grantItemsDto.UserId &&
                item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                // first time this user is getting this item
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };
                await itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                // user already has some of this item, so we add to the quantity
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await itemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}