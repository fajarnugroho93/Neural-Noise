using MessagePipe;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Views;
using SpaceKomodo.Utilities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SpaceKomodo.TurnBasedSystem.Core
{
    public class TurnBasedScope : LifetimeScope
    {
        [SerializeField] private CharacterTurnView _characterTurnViewPrefab;
        [SerializeField] private CurrentTurnSkillView _currentTurnSkillViewPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_characterTurnViewPrefab);
            builder.RegisterInstance(_currentTurnSkillViewPrefab);
            
            builder.RegisterMessageBroker<int>(builder.RegisterMessagePipe());
            
            builder.Register<IViewFactory<CharacterModel, CharacterTurnView>, ViewFactory<CharacterModel, CharacterTurnView>>(Lifetime.Singleton);
            builder.Register<IViewFactory<SkillModel, CurrentTurnSkillView>, ViewFactory<SkillModel, CurrentTurnSkillView>>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<TurnBasedModel>();
            builder.RegisterComponentInHierarchy<TurnBasedView>();
            builder.RegisterComponentInHierarchy<CurrentTurnSelectedCharacterDetailsView>();
            
            builder.RegisterEntryPoint<TurnBasedController>();
        }
    }
}