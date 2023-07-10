using System;
namespace Play.Catalog.Contracts
{
    // ONE RECORD FOR EACH EVENT
    public record CatalogItemCreated(
        Guid ItemId,
        string Name,
        string Description,
        decimal Price
    );
    public record CatalogItemUpdated(
        Guid ItemId,
        string Name,
        string Description,
        decimal Price
    );
    public record CatalogItemDeleted(Guid ItemId);
}