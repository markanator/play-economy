using System;
using Automatonymous;
using Play.Trading.Service.Activities;
using Play.Trading.Service.Contracts;

namespace Play.Trading.Service.StateMachines
{
    public class PurchaseStateMachine : MassTransitStateMachine<PurchaseState>
    {
        public State Accepted { get; }
        public State ItemsGranted { get; }
        public State Completed { get; }
        public State Faulted { get; }

        public Event<PurchaseRequested> PurchaseRequested { get; }
        public Event<GetPurchaseState> GetPurchaseState { get; }

        public PurchaseStateMachine()
        {
            InstanceState(state => state.CurrentState);
            ConfigureEvents();
            ConfigureInitialState();
            ConfigureAny();
        }

        private void ConfigureEvents()
        {
            Event(() => PurchaseRequested);
            Event(() => GetPurchaseState);
        }

        private void ConfigureInitialState()
        {
            Initially(
                When(PurchaseRequested)
                    .Then(context =>
                    {
                        context.Instance.UserId = context.Data.UserId;
                        context.Instance.ItemId = context.Data.ItemId;
                        context.Instance.Quantity = context.Data.Quantity;
                        context.Instance.Received = DateTimeOffset.UtcNow;
                        context.Instance.LastUpdated = context.Instance.Received;
                    })
                    // sum the total price
                    .Activity(x => x.OfType<CalculatePurchaseTotalActivity>())
                    // then we invoke the next activity
                    .TransitionTo(Accepted)
                    // If something goes wrong, we can catch the exception and transition to Faulted state
                    .Catch<Exception>(ex => ex.Then(context =>
                    {
                        context.Instance.ErrorMessage = context.Exception.Message;
                        context.Instance.LastUpdated = DateTimeOffset.UtcNow;
                    })
                    // transition to ERROR state
                    .TransitionTo(Faulted)
                    )
            );
        }

        private void ConfigureAny()
        {
            DuringAny(
                When(GetPurchaseState)
                    .Respond(x => x.Instance)
            );
        }
    }
}