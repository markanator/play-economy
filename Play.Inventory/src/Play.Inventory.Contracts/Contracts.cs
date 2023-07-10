using System;

namespace Play.Inventory.Contracts;

public record GrantItems(
    Guid UserId,
    Guid ItemId,
    int Quantity,
    Guid CorrelationId // id of instance of this message
);

public record InventoryItemsGranted(
    Guid CorrelationId
);

public record SubtractItems(
    Guid UserId,
    Guid ItemId,
    int Quantity,
    Guid CorrelationId // id of instance of this message
);

public record InventoryItemsSubtracted(
    Guid CorrelationId
);

