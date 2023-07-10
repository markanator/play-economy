using System;
using System.Runtime.Serialization;

namespace Play.Trading.Service.Exceptions
{
    [Serializable]
    internal class UnknownItemException : Exception
    {
        public Guid ItemId { get; set; }
        public UnknownItemException(Guid itemId) : base($"Unknown item: {itemId}")
        {
            this.ItemId = itemId;
        }
    }
}