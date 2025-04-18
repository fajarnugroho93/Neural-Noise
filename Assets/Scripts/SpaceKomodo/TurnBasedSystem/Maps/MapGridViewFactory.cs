using System.Collections.Generic;
using SpaceKomodo.TurnBasedSystem.Views;
using UnityEngine;
using VContainer;

namespace SpaceKomodo.TurnBasedSystem.Maps
{
    public class MapGridViewFactory : IMapGridViewFactory
    {
        private readonly MapGridView _prefab;
        private readonly IObjectResolver _resolver;
        private readonly Dictionary<MapGridModel, MapGridView> _views = new();
        
        public MapGridViewFactory(MapGridView prefab, IObjectResolver resolver)
        {
            _prefab = prefab;
            _resolver = resolver;
        }

        public MapGridView Create(MapGridModel model, Transform parent = null)
        {
            if (_views.TryGetValue(model, out var existingView))
            {
                return existingView;
            }
            
            var instance = Object.Instantiate(_prefab, parent);
            _resolver.Inject(instance);
            
            instance.Initialize(model);
            
            _views[model] = instance;
            return instance;
        }
        
        public MapGridView GetView(MapGridModel model)
        {
            return _views.TryGetValue(model, out var view) ? view : null;
        }
        
        public bool TryGetView(MapGridModel model, out MapGridView view)
        {
            return _views.TryGetValue(model, out view);
        }
        
        public void Clear()
        {
            foreach (var view in _views.Values)
            {
                if (view != null)
                {
                    Object.Destroy(view.gameObject);
                }
            }
            
            _views.Clear();
        }
    }
}