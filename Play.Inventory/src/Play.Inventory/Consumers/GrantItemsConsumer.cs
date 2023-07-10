using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Play.Common;
using Play.Inventory.Consumers.Exceptions;
using Play.Inventory.Contracts;
using Play.Inventory.Entities;

namespace Play.Inventory.Consumers
{
    public class GrantItemsConsumer : IConsumer<GrantItems>
    {
        private readonly IRepository<InventoryItem> itemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;

        public GrantItemsConsumer(
            IRepository<InventoryItem> _itemsRepository,
            IRepository<CatalogItem> _catalogItemsRepository
        )
        {
            this.itemsRepository = _itemsRepository;
            this.catalogItemsRepository = _catalogItemsRepository;
        }
        public async Task Consume(ConsumeContext<GrantItems> context)
        {
            var message = context.Message;
            var item = await catalogItemsRepository.GetAsync(message.ItemId);
            if (item == null)
            {
                throw new UnknownItemException(message.ItemId);
            }
            var inventoryItem = await itemsRepository.GetAsync(
                item => item.UserId == message.UserId &&
                item.CatalogItemId == message.ItemId);

            if (inventoryItem == null)
            {
                // first time this user is getting this item
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = message.ItemId,
                    UserId = message.UserId,
                    Quantity = message.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };
                await itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                // user already has some of this item, so we add to the quantity
                inventoryItem.Quantity += message.Quantity;
                await itemsRepository.UpdateAsync(inventoryItem);
            }

            await context.Publish(new InventoryItemsGranted(message.CorrelationId));
        }
    }
}