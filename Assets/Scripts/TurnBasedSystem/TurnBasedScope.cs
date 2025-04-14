using MessagePipe;
using SpaceKomodo.Utilities;
using TurnBasedSystem.Characters;
using TurnBasedSystem.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TurnBasedSystem
{
    public class TurnBasedScope : LifetimeScope
    {
        [SerializeField] private CharacterTurnView characterTurnViewPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(characterTurnViewPrefab);
            
            builder.RegisterMessageBroker<int>(builder.RegisterMessagePipe());
            
            builder.Register<IViewFactory<CharacterModel, CharacterTurnView>, CharacterTurnViewFactory>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<TurnBasedModel>();
            builder.RegisterComponentInHierarchy<TurnBasedView>();
            builder.RegisterComponentInHierarchy<CurrentTurnSelectedCharacterDetailsView>();
            
            builder.RegisterEntryPoint<TurnBasedController>();
        }
    }
}