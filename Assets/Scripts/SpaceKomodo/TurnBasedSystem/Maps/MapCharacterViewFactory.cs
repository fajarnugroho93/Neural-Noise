using System.Collections.Generic;
using SpaceKomodo.TurnBasedSystem.Views;
using UnityEngine;
using VContainer;

namespace SpaceKomodo.TurnBasedSystem.Maps
{
    public class MapCharacterViewFactory : IMapCharacterViewFactory
    {
        private readonly MapCharacterView _prefab;
        private readonly IObjectResolver _resolver;
        private readonly IGridPositionService _gridPositionService;
        private readonly IMapGridViewFactory _mapGridViewFactory;
        private readonly Dictionary<MapCharacterModel, MapCharacterView> _views = new();
        
        public MapCharacterViewFactory(
            MapCharacterView prefab, 
            IObjectResolver resolver, 
            IGridPositionService gridPositionService,
            IMapGridViewFactory mapGridViewFactory)
        {
            _prefab = prefab;
            _resolver = resolver;
            _gridPositionService = gridPositionService;
            _mapGridViewFactory = mapGridViewFactory;
        }

        public MapCharacterView Create(MapCharacterModel model, Transform parent = null)
        {
            if (_views.TryGetValue(model, out var existingView))
            {
                return existingView;
            }
            
            if (_mapGridViewFactory.TryGetView(model.MapPositions, out var gridView))
            {
                parent = gridView.transform;
            }
            
            var instance = Object.Instantiate(_prefab, parent);
            _resolver.Inject(instance);
            
            instance.Initialize(model);
            
            if (parent == null)
            {
                instance.transform.position = _gridPositionService.GetLocalPosition(model.MapPositions);
            }
            else
            {
                instance.transform.localPosition = Vector3.zero;
            }
            
            _views[model] = instance;
            return instance;
        }
        
        public MapCharacterView GetView(MapCharacterModel model)
        {
            return _views.TryGetValue(model, out var view) ? view : null;
        }
        
        public bool TryGetView(MapCharacterModel model, out MapCharacterView view)
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