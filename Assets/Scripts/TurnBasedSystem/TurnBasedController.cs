using System.Collections.Generic;
using MessagePipe;
using ObservableCollections;
using R3;
using SpaceKomodo.Utilities;
using TurnBasedSystem.Characters;
using TurnBasedSystem.Events;
using TurnBasedSystem.Views;
using UnityEngine;
using VContainer.Unity;

namespace TurnBasedSystem
{
    public class TurnBasedController : IStartable
    {
        private readonly TurnBasedModel _model;
        private readonly TurnBasedView _view;
        private readonly CurrentTurnSelectedCharacterDetailsView _currentTurnSelectedCharacterDetailsView;
        private readonly ISubscriber<CurrentTurnCharacterSelectedEvent> _currentTurnCharacterSelectedSubscriber;
        private readonly ISubscriber<NextTurnButtonClickedEvent> _nextTurnButtonClickedSubscriber;
        private readonly IViewFactory<CharacterModel, CharacterTurnView> _characterViewFactory;
        
        private readonly Dictionary<CharacterModel, CharacterTurnView> _characterViews = new();
        
        public TurnBasedController(
            TurnBasedModel model, 
            TurnBasedView view,
            CurrentTurnSelectedCharacterDetailsView currentTurnSelectedCharacterDetailsView,
            ISubscriber<CurrentTurnCharacterSelectedEvent> currentTurnCharacterSelectedSubscriber,
            ISubscriber<NextTurnButtonClickedEvent> nextTurnButtonClickedSubscriber,
            IViewFactory<CharacterModel, CharacterTurnView> characterViewFactory)
        {
            _model = model;
            _view = view;
            _currentTurnSelectedCharacterDetailsView = currentTurnSelectedCharacterDetailsView;
            _currentTurnCharacterSelectedSubscriber = currentTurnCharacterSelectedSubscriber;
            _nextTurnButtonClickedSubscriber = nextTurnButtonClickedSubscriber;
            _characterViewFactory = characterViewFactory;
        }
        
        public void Start()
        {
            _model.TurnOrderChanged.Subscribe(_ => UpdateViewOrder());
            
            _model.models.ObserveAdd().Select(addEvent => addEvent.Value).Subscribe(OnModelAdded);
            _model.models.ObserveRemove().Select(removeEvent => removeEvent.Value).Subscribe(OnModelRemoved);
            
            _nextTurnButtonClickedSubscriber.Subscribe(_ => OnNextTurnButtonClicked());
            
            void OnModelAdded(CharacterModel newModel)
            {
                var view = _characterViewFactory.Create(newModel, _view.characterTurnParentViewTransform);
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

            _currentTurnCharacterSelectedSubscriber.Subscribe(evt => OnCurrentTurnCharacterSelected(evt.CharacterModel));

            void OnCurrentTurnCharacterSelected(CharacterModel characterModel)
            {
                _currentTurnSelectedCharacterDetailsView.SetCharacterModel(characterModel);
            }
            
            _model.BeginBattle();
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