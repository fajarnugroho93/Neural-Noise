using System.Collections.Generic;
using MessagePipe;
using ObservableCollections;
using R3;
using SpaceKomodo.Extensions;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Commands;
using SpaceKomodo.TurnBasedSystem.Events;
using SpaceKomodo.TurnBasedSystem.Views;
using SpaceKomodo.Utilities;
using UnityEngine;
using VContainer.Unity;
using DisposableBag = R3.DisposableBag;

namespace SpaceKomodo.TurnBasedSystem.Core
{
    public class TurnBasedController : IStartable
    {
        private readonly TurnBasedModel _model;
        private readonly TurnBasedView _view;
        private readonly CurrentTurnSelectedCharacterDetailsView _currentTurnSelectedCharacterDetailsView;
        private readonly ISubscriber<CurrentTurnCharacterSelectedEvent> _currentTurnCharacterSelectedSubscriber;
        private readonly ISubscriber<NextTurnButtonClickedEvent> _nextTurnButtonClickedSubscriber;
        private readonly IViewFactory<CharacterModel, CharacterTurnView> _characterViewFactory;
        private readonly IViewFactory<SkillModel, CurrentTurnSkillView> _currentTurnSkillViewFactory;
        private readonly SkillExecutor _skillExecutor;
        private readonly ISubscriber<CommandExecutedEvent> _commandExecutedSubscriber;
        private readonly ISubscriber<EffectExecutedEvent> _effectExecutedSubscriber;

        private readonly Dictionary<CharacterModel, CharacterTurnView> _characterViews = new();
        private DisposableBag _disposableBag;
        
        public TurnBasedController(
            TurnBasedModel model, 
            TurnBasedView view,
            CurrentTurnSelectedCharacterDetailsView currentTurnSelectedCharacterDetailsView,
            ISubscriber<CurrentTurnCharacterSelectedEvent> currentTurnCharacterSelectedSubscriber,
            ISubscriber<NextTurnButtonClickedEvent> nextTurnButtonClickedSubscriber,
            IViewFactory<CharacterModel, CharacterTurnView> characterViewFactory,
            IViewFactory<SkillModel, CurrentTurnSkillView> currentTurnSkillViewFactory,
            SkillExecutor skillExecutor,
            ISubscriber<CommandExecutedEvent> commandExecutedSubscriber,
            ISubscriber<EffectExecutedEvent> effectExecutedSubscriber)
        {
            _model = model;
            _view = view;
            _currentTurnSelectedCharacterDetailsView = currentTurnSelectedCharacterDetailsView;
            _currentTurnCharacterSelectedSubscriber = currentTurnCharacterSelectedSubscriber;
            _nextTurnButtonClickedSubscriber = nextTurnButtonClickedSubscriber;
            _characterViewFactory = characterViewFactory;
            _currentTurnSkillViewFactory = currentTurnSkillViewFactory;
            _skillExecutor = skillExecutor;
            _commandExecutedSubscriber = commandExecutedSubscriber;
            _effectExecutedSubscriber = effectExecutedSubscriber;
        }
        
        public void Start()
        {
            _commandExecutedSubscriber.Subscribe(OnCommandExecuted);
            _effectExecutedSubscriber.Subscribe(OnEffectExecuted);
            
            _model.TurnOrderChanged.Subscribe(_ => UpdateViewOrder());
            
            _model.models.ObserveAdd().Select(addEvent => addEvent.Value).Subscribe(OnModelAdded);
            _model.models.ObserveRemove().Select(removeEvent => removeEvent.Value).Subscribe(OnModelRemoved);
            
            _nextTurnButtonClickedSubscriber.Subscribe(_ => OnNextTurnButtonClicked());
            
            void OnModelAdded(CharacterModel newModel)
            {
                var view = _characterViewFactory.Create(newModel, _view.CharacterTurnViewParentTransform);
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
                
                _disposableBag.Dispose();
                _disposableBag = new DisposableBag();
                
                foreach (var skillModel in characterModel.Skills)
                {
                    var view = _currentTurnSkillViewFactory.Create(skillModel, _view.CurrentTurnSkillViewParentTransform);
                    view.gameObject.ToDisposable().AddTo(ref _disposableBag);
                }
            }
        }
        
        private void OnCommandExecuted(CommandExecutedEvent evt)
        {
            if (evt.Command is SkillCommand skillCommand)
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
        
        private void OnEffectExecuted(EffectExecutedEvent evt)
        {
            UpdateCharacterViews();
        }
        
        private void UpdateCharacterViews()
        {
            foreach (var characterView in _characterViews.Values)
            {
                characterView.transform.SetSiblingIndex(characterView.GetComponent<CharacterTurnView>().GetComponentInParent<Transform>().GetSiblingIndex());
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