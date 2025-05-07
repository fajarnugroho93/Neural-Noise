using MessagePipe;
using R3;
using SpaceKomodo.TurnBasedSystem.Core;
using SpaceKomodo.TurnBasedSystem.Events;
using VContainer.Unity;
using DisposableBag = R3.DisposableBag;

namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public class TurnCommandController : IStartable
    {
        private readonly TurnBasedModel _turnModel;

        private readonly IPublisher<CommandExecutedEvent> _commandExecutedPublisher;
        
        private readonly ISubscriber<SkillClickedEvent> _skillClickedSubscriber;
        private readonly ISubscriber<TargetClickedEvent> _targetClickedSubscriber;
        private readonly ISubscriber<DiceClickedEvent> _diceClickedSubscriber;
        private readonly ISubscriber<ExecuteCommandEvent> _executeCommandSubscriber;
        private readonly ISubscriber<CancelCommandEvent> _cancelCommandSubscriber;
        private readonly ISubscriber<CurrentTurnCharacterSelectedEvent> _currentTurnCharacterSelectedSubscriber;
        
        private DisposableBag _disposableBag;
        
        public TurnCommandController(
            TurnBasedModel turnModel,
            IPublisher<CommandExecutedEvent> commandExecutedPublisher,
            ISubscriber<SkillClickedEvent> skillClickedSubscriber,
            ISubscriber<TargetClickedEvent> targetClickedSubscriber,
            ISubscriber<DiceClickedEvent> diceClickedSubscriber,
            ISubscriber<ExecuteCommandEvent> executeCommandSubscriber,
            ISubscriber<CancelCommandEvent> cancelCommandSubscriber,
            ISubscriber<CurrentTurnCharacterSelectedEvent> currentTurnCharacterSelectedSubscriber)
        {
            _turnModel = turnModel;
            
            _commandExecutedPublisher = commandExecutedPublisher;
            
            _skillClickedSubscriber = skillClickedSubscriber;
            _targetClickedSubscriber = targetClickedSubscriber;
            _diceClickedSubscriber = diceClickedSubscriber;
            _executeCommandSubscriber = executeCommandSubscriber;
            _cancelCommandSubscriber = cancelCommandSubscriber;
            _currentTurnCharacterSelectedSubscriber = currentTurnCharacterSelectedSubscriber;
        }
        
        public void Start()
        {
            _skillClickedSubscriber.Subscribe(OnSkillClicked).AddTo(ref _disposableBag);
            _targetClickedSubscriber.Subscribe(OnTargetClicked).AddTo(ref _disposableBag);
            _diceClickedSubscriber.Subscribe(OnDiceClicked).AddTo(ref _disposableBag);
            _executeCommandSubscriber.Subscribe(_ => ExecuteCommand()).AddTo(ref _disposableBag);
            _cancelCommandSubscriber.Subscribe(_ => CancelCommand()).AddTo(ref _disposableBag);
            _currentTurnCharacterSelectedSubscriber.Subscribe(evt => _turnModel.SetCurrentCharacter(evt.CharacterModel)).AddTo(ref _disposableBag);
            
            _turnModel.CurrentPhase.Subscribe(OnPhaseChanged).AddTo(ref _disposableBag);
        }
        
        private void OnDiceClicked(DiceClickedEvent evt)
        {
            if (_turnModel.CurrentPhase.Value != TurnPhase.SelectDice
                && _turnModel.CurrentPhase.Value != TurnPhase.SelectSkill 
                && _turnModel.CurrentPhase.Value != TurnPhase.SelectTarget
                && _turnModel.CurrentPhase.Value != TurnPhase.Confirmation)
            {
                return;
            }

            if (!evt.DiceModel.IsSelectable.Value)
            {
                return;
            }
            
            _turnModel.SetSelectedDice(evt.DiceModel);
        }
        
        private void OnSkillClicked(SkillClickedEvent evt)
        {
            if (_turnModel.CurrentPhase.Value != TurnPhase.SelectSkill 
                && _turnModel.CurrentPhase.Value != TurnPhase.SelectTarget
                && _turnModel.CurrentPhase.Value != TurnPhase.Confirmation)
            {
                return;
            }

            if (!evt.Skill.IsSelectable.Value)
            {
                return;
            }

            _turnModel.SetSelectedSkill(evt.Skill);
        }
        
        private void OnTargetClicked(TargetClickedEvent evt)
        {
            if (_turnModel.CurrentPhase.Value != TurnPhase.SelectTarget && _turnModel.CurrentPhase.Value != TurnPhase.Confirmation)
            {
                return;
            }
    
            if (_turnModel.IsValidTarget(evt.Target))
            {
                _turnModel.SetSelectedTarget(evt.Target);
            }
        }
        
        private void ExecuteCommand()
        {
            if (_turnModel.CurrentPhase.Value != TurnPhase.Confirmation || _turnModel.CurrentCommand == null)
            {
                return;
            }
    
            _turnModel.ExecuteCurrentCommand();
        }
        
        private void CancelCommand()
        {
            switch (_turnModel.CurrentPhase.Value)
            {
                case TurnPhase.SelectDice:
                    _turnModel.CancelSelectDice();
                    break;
                case TurnPhase.SelectSkill:
                    _turnModel.CancelSelectSkill();
                    break;
                case TurnPhase.SelectTarget:
                    _turnModel.CancelSelectTarget();
                    break;
                case TurnPhase.Confirmation:
                    _turnModel.CancelConfirmation();
                    break;
            }
        }
        
        private void OnPhaseChanged(TurnPhase phase)
        {
            switch (phase)
            {
                case TurnPhase.SelectDice:
                    _turnModel.ClearSelectedTargets();
                    _turnModel.ClearValidTargets();
                    _turnModel.ClearSelectedSkills();
                    _turnModel.ClearValidSkills();
                    _turnModel.ClearSelectedDice();
                    break;
                case TurnPhase.SelectSkill:
                    _turnModel.ClearSelectedTargets();
                    _turnModel.ClearValidTargets();
                    _turnModel.ClearSelectedSkills();
                    _turnModel.ClearValidSkills();
                    break;
                case TurnPhase.SelectTarget:
                    break;
                case TurnPhase.Execute:
                    if (_turnModel.CurrentCommand != null && _turnModel.CurrentCommand.CanExecute())
                    {
                        _commandExecutedPublisher.Publish(new CommandExecutedEvent(_turnModel.CurrentCommand));
                    }
                    break;
            }
        }
    }
}