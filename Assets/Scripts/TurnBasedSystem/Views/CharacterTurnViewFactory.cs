using SpaceKomodo.Utilities;
using TurnBasedSystem.Characters;
using UnityEngine;
using VContainer;

namespace TurnBasedSystem.Views
{
    public class CharacterTurnViewFactory : IViewFactory<CharacterModel, CharacterTurnView>
    {
        private readonly CharacterTurnView _prefab;
        private readonly IObjectResolver _resolver;
        
        [Inject]
        public CharacterTurnViewFactory(CharacterTurnView prefab, IObjectResolver resolver)
        {
            _prefab = prefab;
            _resolver = resolver;
        }
        
        public CharacterTurnView Create(CharacterModel model, Transform parent = null)
        {
            var instance = Object.Instantiate(_prefab, parent);
            _resolver.Inject(instance);
            instance.Initialize(model);
            return instance;
        }
    }
}