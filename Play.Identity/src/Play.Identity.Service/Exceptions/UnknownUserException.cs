using System;
using System.Runtime.Serialization;

namespace Play.Identity.Service.Exceptions
{
    [Serializable]
    internal class UnknownUserException : Exception
    {
        public Guid userId { get; }

        public UnknownUserException(Guid userId) : base($"Unknown user with id {userId}")
        {
            this.userId = userId;
        }
    }
}