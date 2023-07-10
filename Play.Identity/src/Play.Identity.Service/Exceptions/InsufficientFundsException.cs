using System;
using System.Runtime.Serialization;

namespace Play.Identity.Service.Exceptions
{
    [Serializable]
    internal class InsufficientFundsException : Exception
    {
        private Guid UserId { get; }
        private decimal AmountToDebit { get; }
        public InsufficientFundsException(Guid userId, decimal amount)
        : base($"Insufficient funds for user with id {userId} to debit {amount}")
        {
            this.UserId = userId;
            this.AmountToDebit = amount;
        }
    }
}