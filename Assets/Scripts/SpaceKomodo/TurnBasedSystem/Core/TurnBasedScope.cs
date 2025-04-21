using MessagePipe;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Commands;
using SpaceKomodo.TurnBasedSystem.Maps;
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
        [SerializeField] private MapGridView _mapGridViewPrefab;
        [SerializeField] private MapCharacterView _mapCharacterViewPrefab;
        [SerializeField] private TargetIndicatorView _targetIndicatorPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_characterTurnViewPrefab);
            builder.RegisterInstance(_currentTurnSkillViewPrefab);
            builder.RegisterInstance(_mapGridViewPrefab);
            builder.RegisterInstance(_mapCharacterViewPrefab);
            builder.RegisterInstance(_targetIndicatorPrefab);
            
            builder.RegisterMessageBroker<int>(builder.RegisterMessagePipe());
            
            builder.Register<IViewFactory<CharacterModel, CharacterTurnView>, ViewFactory<CharacterModel, CharacterTurnView>>(Lifetime.Singleton);
            builder.Register<IViewFactory<SkillModel, CurrentTurnSkillView>, ViewFactory<SkillModel, CurrentTurnSkillView>>(Lifetime.Singleton);
            builder.Register<IMapGridViewFactory, MapGridViewFactory>(Lifetime.Singleton);
            builder.Register<IMapCharacterViewFactory, MapCharacterViewFactory>(Lifetime.Singleton);
            builder.Register<IGridPositionService, GridPositionService>(Lifetime.Singleton);
            builder.Register<ITargetIndicatorFactory, TargetIndicatorFactory>(Lifetime.Singleton);
            builder.Register<ITargetIndicatorManager, TargetIndicatorManager>(Lifetime.Singleton);

            ServiceRegistration.RegisterEffectSystem(builder);

            builder.RegisterComponentInHierarchy<TurnBasedModel>();
            builder.RegisterComponentInHierarchy<TurnBasedView>();
            builder.RegisterComponentInHierarchy<CurrentTurnSelectedCharacterDetailsView>();
            
            builder.RegisterEntryPoint<TurnBasedController>();
            builder.RegisterEntryPoint<TurnCommandController>();
            builder.RegisterEntryPoint<MapController>();
        }
    }
}