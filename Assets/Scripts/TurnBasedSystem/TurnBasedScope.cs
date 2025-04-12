using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace TurnBasedSystem
{
    public class TurnBasedScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessageBroker<int>(builder.RegisterMessagePipe());
            
            builder.RegisterEntryPoint<TurnBasedController>();
            
            builder.RegisterComponentInHierarchy<TurnBasedModel>();
            builder.RegisterComponentInHierarchy<TurnBasedView>();
        }
    }
}