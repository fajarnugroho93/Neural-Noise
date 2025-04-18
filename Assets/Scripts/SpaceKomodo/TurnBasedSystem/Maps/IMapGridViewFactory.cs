using SpaceKomodo.TurnBasedSystem.Views;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Maps
{
    public interface IMapGridViewFactory
    {
        MapGridView Create(MapGridModel model, Transform parent = null);
        MapGridView GetView(MapGridModel model);
        bool TryGetView(MapGridModel model, out MapGridView view);
        void Clear();
    }
}