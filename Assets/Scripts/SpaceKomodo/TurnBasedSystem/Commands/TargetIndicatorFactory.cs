using SpaceKomodo.TurnBasedSystem.Maps;
using SpaceKomodo.TurnBasedSystem.Views;
using UnityEngine;
using VContainer;

namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public class TargetIndicatorFactory : ITargetIndicatorFactory
    {
        private readonly TargetIndicatorView _prefab;
        private readonly IObjectResolver _resolver;
        private readonly IMapCharacterViewFactory _mapCharacterViewFactory;
        
        public TargetIndicatorFactory(
            TargetIndicatorView prefab,
            IObjectResolver resolver,
            IMapCharacterViewFactory mapCharacterViewFactory)
        {
            _prefab = prefab;
            _resolver = resolver;
            _mapCharacterViewFactory = mapCharacterViewFactory;
        }
        
        public TargetIndicatorView Create(MapCharacterModel model)
        {
            if (_mapCharacterViewFactory.TryGetView(model, out var characterView))
            {
                var instance = Object.Instantiate(_prefab, characterView.transform);
                _resolver.Inject(instance);
                instance.Initialize(model);
                return instance;
            }
            
            return null;
        }
    }
}