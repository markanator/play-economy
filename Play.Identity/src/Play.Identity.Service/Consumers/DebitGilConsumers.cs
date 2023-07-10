using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Play.Identity.Service.Entities;
using Play.Identity.Service.Exceptions;
using static Play.Identity.Contracts.Contracts;

namespace Play.Identity.Service.Consumers
{
    public class DebitGilConsumers : IConsumer<DebitGil>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DebitGilConsumers(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task Consume(ConsumeContext<DebitGil> context)
        {
            var message = context.Message;
            var user = await _userManager.FindByIdAsync(message.UserId.ToString());
            if (user is null)
            {
                throw new UnknownUserException(message.UserId);
            }
            user.Gil -= message.Amount;
            // check if negative, if true, throw exception
            if (user.Gil < 0)
            {
                throw new InsufficientFundsException(message.UserId, message.Amount);
            }
            // all good, update user
            await _userManager.UpdateAsync(user);
            await context.Publish(new GilDebited(message.CorrelationId));
        }
    }
}