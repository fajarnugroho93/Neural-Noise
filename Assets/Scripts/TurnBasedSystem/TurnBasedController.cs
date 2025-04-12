using System.Collections.Generic;
using MessagePipe;
using ObservableCollections;
using R3;
using TurnBasedSystem.Characters;
using TurnBasedSystem.Events;
using UnityEngine;
using VContainer.Unity;

namespace TurnBasedSystem
{
    public class TurnBasedController : IStartable
    {
        private readonly TurnBasedModel _model;
        private readonly TurnBasedView _view;
        private readonly ISubscriber<NextTurnButtonClickedEvent> _nextTurnButtonClickedSubscriber;
        
        private readonly Dictionary<CharacterModel, CharacterTurnView> _characterViews = new();
        
        public TurnBasedController(
            TurnBasedModel model, 
            TurnBasedView view,
            ISubscriber<NextTurnButtonClickedEvent> nextTurnButtonClickedSubscriber)
        {
            _model = model;
            _view = view;
            _nextTurnButtonClickedSubscriber = nextTurnButtonClickedSubscriber;
        }
        
        public void Start()
        {
            var viewPrefab = _view.characterTurnViewPrefab;
            
            _model.TurnOrderChanged.Subscribe(_ => UpdateViewOrder());
            
            _model.models.ObserveAdd().Select(addEvent => addEvent.Value).Subscribe(OnModelAdded);
            _model.models.ObserveRemove().Select(removeEvent => removeEvent.Value).Subscribe(OnModelRemoved);
            
            _model.BeginBattle();
            _nextTurnButtonClickedSubscriber.Subscribe(_ => OnNextTurnButtonClicked());
            
            void OnModelAdded(CharacterModel newModel)
            {
                var view = Object.Instantiate(viewPrefab, _view.characterTurnParentViewTransform);
                view.Initialize(newModel);
                _characterViews.Add(newModel, view);
            }
            
            void OnModelRemoved(CharacterModel removedModel)
            {
                if (_characterViews.TryGetValue(removedModel, out var view))
                {
                    Object.Destroy(view.gameObject);
                    _characterViews.Remove(removedModel);
                }
            }
        }
        
        private void UpdateViewOrder()
        {
            var sortedModels = new List<CharacterModel>(_model.models);
            sortedModels.Sort((a, b) => a.TurnOrder.Value.CompareTo(b.TurnOrder.Value));
            
            for (var ii = 0; ii < sortedModels.Count; ++ii)
            {
                if (_characterViews.TryGetValue(sortedModels[ii], out var view))
                {
                    view.transform.SetSiblingIndex(ii);
                }
            }
        }

        private void OnNextTurnButtonClicked()
        {
            if (_model.HasNextTurn())
            {
                _model.NextTurn();
            }
            else
            {
                _model.NextRound();
            }
        }
    }
}