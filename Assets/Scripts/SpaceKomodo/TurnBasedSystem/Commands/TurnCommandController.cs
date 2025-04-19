using MessagePipe;
using R3;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Core;
using SpaceKomodo.TurnBasedSystem.Events;
using VContainer.Unity;
using DisposableBag = R3.DisposableBag;
using ExecuteCommandEvent = SpaceKomodo.TurnBasedSystem.Events.ExecuteCommandEvent;

namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public class TurnCommandController : IStartable
    {
        private readonly TurnBasedModel _turnModel;
        private readonly IEffectExecutor _effectExecutor;
        private readonly ITargetSelector _targetSelector;

        private readonly IPublisher<SkillSelectedEvent> _skillSelectedPublisher;
        private readonly IPublisher<TargetSelectedEvent> _targetSelectedPublisher;
        private readonly IPublisher<CommandExecutedEvent> _commandExecutedPublisher;
        
        private readonly ISubscriber<SkillClickedEvent> _skillClickedSubscriber;
        private readonly ISubscriber<TargetClickedEvent> _targetClickedSubscriber;
        private readonly ISubscriber<ExecuteCommandEvent> _executeCommandSubscriber;
        private readonly ISubscriber<CancelCommandEvent> _cancelCommandSubscriber;
        private readonly ISubscriber<CurrentTurnCharacterSelectedEvent> _currentTurnCharacterSelectedSubscriber;
        
        private DisposableBag _disposableBag;
        
        public TurnCommandController(
            TurnBasedModel turnModel,
            IEffectExecutor effectExecutor,
            ITargetSelector targetSelector,
            IPublisher<SkillSelectedEvent> skillSelectedPublisher,
            IPublisher<TargetSelectedEvent> targetSelectedPublisher,
            IPublisher<CommandExecutedEvent> commandExecutedPublisher,
            ISubscriber<SkillClickedEvent> skillClickedSubscriber,
            ISubscriber<TargetClickedEvent> targetClickedSubscriber,
            ISubscriber<ExecuteCommandEvent> executeCommandSubscriber,
            ISubscriber<CancelCommandEvent> cancelCommandSubscriber,
            ISubscriber<CurrentTurnCharacterSelectedEvent> currentTurnCharacterSelectedSubscriber)
        {
            _turnModel = turnModel;
            _effectExecutor = effectExecutor;
            _targetSelector = targetSelector;
            
            _skillSelectedPublisher = skillSelectedPublisher;
            _targetSelectedPublisher = targetSelectedPublisher;
            _commandExecutedPublisher = commandExecutedPublisher;
            
            _skillClickedSubscriber = skillClickedSubscriber;
            _targetClickedSubscriber = targetClickedSubscriber;
            _executeCommandSubscriber = executeCommandSubscriber;
            _cancelCommandSubscriber = cancelCommandSubscriber;
            _currentTurnCharacterSelectedSubscriber = currentTurnCharacterSelectedSubscriber;
        }
        
        public void Start()
        {
            _skillClickedSubscriber.Subscribe(OnSkillClicked).AddTo(ref _disposableBag);
            _targetClickedSubscriber.Subscribe(OnTargetClicked).AddTo(ref _disposableBag);
            _executeCommandSubscriber.Subscribe(_ => ExecuteCommand()).AddTo(ref _disposableBag);
            _cancelCommandSubscriber.Subscribe(_ => CancelCommand()).AddTo(ref _disposableBag);
            _currentTurnCharacterSelectedSubscriber.Subscribe(evt => SetCurrentCharacter(evt.CharacterModel)).AddTo(ref _disposableBag);
            
            _turnModel.CurrentPhase.Subscribe(OnPhaseChanged).AddTo(ref _disposableBag);
        }
        
        private void SetCurrentCharacter(CharacterModel character)
        {
            _turnModel.SetCurrentCharacter(character);
        }
        
        private void OnSkillClicked(SkillClickedEvent evt)
        {
            if (_turnModel.CurrentPhase.Value != TurnPhase.SelectSkill && _turnModel.CurrentPhase.Value != TurnPhase.SelectTarget)
            {
                return;
            }

            _targetSelector.ClearValidTargets();
            _turnModel.SetSelectedSkill(evt.Skill);
            _skillSelectedPublisher.Publish(new SkillSelectedEvent(_turnModel.SelectedSkill));
            _targetSelector.SetValidTargets(_turnModel.CurrentCharacter, _turnModel.SelectedSkill);
        }
        
        private void OnTargetClicked(TargetClickedEvent evt)
        {
            if (_turnModel.CurrentPhase.Value != TurnPhase.SelectTarget)
            {
                return;
            }
    
            if (_targetSelector.IsValidTarget(evt.Target))
            {
                _turnModel.SetSelectedTarget(evt.Target);
                _targetSelectedPublisher.Publish(new TargetSelectedEvent(_turnModel.SelectedTarget));
        
                _turnModel.SetCurrentCommand(new SkillCommand(_turnModel.CurrentCharacter, _turnModel.SelectedSkill, _turnModel.SelectedTarget, _effectExecutor));
            }
        }
        
        private void ExecuteCommand()
        {
            if (_turnModel.CurrentPhase.Value != TurnPhase.Confirmation || _turnModel.CurrentCommand == null)
            {
                return;
            }
    
            _turnModel.ExecuteCurrentCommand();
    
            if (_turnModel.CurrentCommand.CanExecute())
            {
                _turnModel.CurrentCommand.Execute();
                _targetSelector.ClearValidTargets();
                _commandExecutedPublisher.Publish(new CommandExecutedEvent(_turnModel.CurrentCommand));
            }
        }
        
        private void CancelCommand()
        {
            if (_turnModel.CurrentPhase.Value == TurnPhase.SelectTarget)
            {
                _turnModel.CancelSelectTarget();
                _targetSelector.ClearValidTargets();
            }
            else if (_turnModel.CurrentPhase.Value == TurnPhase.Confirmation)
            {
                _turnModel.CancelConfirmation();
            }
        }
        
        private void OnPhaseChanged(TurnPhase phase)
        {
            switch (phase)
            {
                case TurnPhase.Idle:
                    _turnModel.ResetTurnState();
                    _targetSelector.ClearValidTargets();
                    break;
                case TurnPhase.NextTurn:
                    _turnModel.CurrentPhase.Value = TurnPhase.Idle;
                    break;
            }
        }
    }
}