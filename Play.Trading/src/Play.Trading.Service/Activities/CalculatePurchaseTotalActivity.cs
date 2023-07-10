using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;
using Play.Common;
using Play.Trading.Service.Exceptions;
using Play.Trading.Service.Contracts;
using Play.Trading.Service.Entities;
using Play.Trading.Service.StateMachines;

namespace Play.Trading.Service.Activities
{
    // StateMachine class
    // Event that provides information to activity
    public class CalculatePurchaseTotalActivity : Activity<PurchaseState, PurchaseRequested>
    {
        private readonly IRepository<CatalogItem> _repository;

        public CalculatePurchaseTotalActivity(IRepository<CatalogItem> repository)
        {
            _repository = repository;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            // just to comply with MassTransit
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<PurchaseState, PurchaseRequested> context, Behavior<PurchaseState, PurchaseRequested> next)
        {
            // data from event before this activity
            var message = context.Data;
            // get item from catalog repository
            var item = await _repository.GetAsync(message.ItemId);
            // the item was not found, throw exception
            if (item is null)
            {
                throw new UnknownItemException(message.ItemId);
            }
            // usually another microservice will be responsible for calculating the price
            context.Instance.PurchaseTotal = item.Price * message.Quantity;
            context.Instance.LastUpdated = DateTime.UtcNow;

            // invoke next activity
            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<PurchaseState, PurchaseRequested, TException> context, Behavior<PurchaseState, PurchaseRequested> next) where TException : Exception
        {
            // keep moving forward
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            // just to comply with MassTransit, lets us add some metadata to the activity
            context.CreateScope("calculate-purchase-total");
        }
    }
}