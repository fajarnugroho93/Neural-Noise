using SpaceKomodo.TurnBasedSystem.Views;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Maps
{
    public interface IMapCharacterViewFactory
    {
        MapCharacterView Create(MapCharacterModel model, Transform parent = null);
        MapCharacterView GetView(MapCharacterModel model);
        bool TryGetView(MapCharacterModel model, out MapCharacterView view);
        void Clear();
    }
}