using System.Collections.Generic;
using MessagePipe;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Core;
using SpaceKomodo.TurnBasedSystem.Events;
using SpaceKomodo.TurnBasedSystem.Views;
using UnityEngine;
using VContainer.Unity;

namespace SpaceKomodo.TurnBasedSystem.Maps
{
    public class MapController : IStartable
    {
        private readonly TurnBasedModel _model;
        private readonly TurnBasedView _view;
        private readonly IMapGridViewFactory _mapGridViewFactory;
        private readonly IMapCharacterViewFactory _mapCharacterViewFactory;
        private readonly ISubscriber<CurrentTurnCharacterSelectedEvent> _currentTurnCharacterSelectedSubscriber;
        private readonly IGridPositionService _gridPositionService;

        private readonly Dictionary<CharacterModel, MapCharacterView> _characterViews = new();
        private MapCharacterView _selectedCharacterView;
        
        public MapController(
            TurnBasedModel model,
            TurnBasedView view,
            IMapGridViewFactory mapGridViewFactory,
            IMapCharacterViewFactory mapCharacterViewFactory,
            ISubscriber<CurrentTurnCharacterSelectedEvent> currentTurnCharacterSelectedSubscriber,
            IGridPositionService gridPositionService)
        {
            _model = model;
            _view = view;
            _mapGridViewFactory = mapGridViewFactory;
            _mapCharacterViewFactory = mapCharacterViewFactory;
            _currentTurnCharacterSelectedSubscriber = currentTurnCharacterSelectedSubscriber;
            _gridPositionService = gridPositionService;
        }
        
        public void Start()
        {
            _currentTurnCharacterSelectedSubscriber.Subscribe(evt => OnCurrentTurnCharacterSelected(evt.CharacterModel));
            
            _model.SetupBattle();
            
            CreateGridViews(_model.MapModel.HeroMapGrids, _view.HeroGridParentTransform);
            CreateGridViews(_model.MapModel.EnemyMapGrids, _view.EnemyGridParentTransform);
            
            CreateCharacterViews(_model.heroMapModels, _view.HeroGridParentTransform);
            CreateCharacterViews(_model.enemyMapModels, _view.EnemyGridParentTransform);
            
            _model.BeginBattle();
        }
        
        private void CreateGridViews(MapGridModel[,] grids, Transform parent)
        {
            var columns = grids.GetLength(0);
            var rows = grids.GetLength(1);
            
            for (var x = 0; x < columns; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    var gridModel = grids[x, y];
                    var gridView = _mapGridViewFactory.Create(gridModel, parent);
                    
                    gridView.transform.localPosition = _gridPositionService.GetLocalPosition(gridModel);
                }
            }
        }
        
        private void CreateCharacterViews(List<MapCharacterModel> mapCharacterModels, Transform parent)
        {
            foreach (var mapCharacterModel in mapCharacterModels)
            {
                var characterView = _mapCharacterViewFactory.Create(mapCharacterModel, parent);
                _characterViews[mapCharacterModel.CharacterModel] = characterView;
            }
        }
        
        private void OnCurrentTurnCharacterSelected(CharacterModel characterModel)
        {
            // if (_selectedCharacterView != null)
            // {
            //     _selectedCharacterView.SetSelected(false);
            // }
            //
            // if (_characterViews.TryGetValue(characterModel, out var characterView))
            // {
            //     characterView.SetSelected(true);
            //     _selectedCharacterView = characterView;
            // }
        }
    }
}