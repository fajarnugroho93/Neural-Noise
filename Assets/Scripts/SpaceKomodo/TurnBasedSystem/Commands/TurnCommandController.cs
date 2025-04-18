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
        
        private readonly ReactiveProperty<TurnPhase> _currentPhase = new(TurnPhase.Idle);
        public Observable<TurnPhase> CurrentPhase => _currentPhase;
        
        private CharacterModel _currentCharacter;
        private SkillModel _selectedSkill;
        private CharacterModel _selectedTarget;
        private TurnCommand _currentCommand;
        
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
            
            _currentPhase.Subscribe(OnPhaseChanged).AddTo(ref _disposableBag);
        }
        
        private void SetCurrentCharacter(CharacterModel character)
        {
            _currentCharacter = character;
            _currentPhase.Value = TurnPhase.SelectSkill;
        }
        
        private void OnSkillClicked(SkillClickedEvent evt)
        {
            if (_currentPhase.Value != TurnPhase.SelectSkill) return;
            
            _selectedSkill = evt.Skill;
            _skillSelectedPublisher.Publish(new SkillSelectedEvent(_selectedSkill));
            
            _currentPhase.Value = TurnPhase.SelectTarget;
            
            _targetSelector.SetValidTargets(_currentCharacter, _selectedSkill);
        }
        
        private void OnTargetClicked(TargetClickedEvent evt)
        {
            if (_currentPhase.Value != TurnPhase.SelectTarget) return;
            
            if (_targetSelector.IsValidTarget(evt.Target))
            {
                _selectedTarget = evt.Target;
                _targetSelectedPublisher.Publish(new TargetSelectedEvent(_selectedTarget));
                
                _currentCommand = new SkillCommand(_currentCharacter, _selectedSkill, _selectedTarget, _effectExecutor);
            }
        }
        
        private void ExecuteCommand()
        {
            if (_currentPhase.Value != TurnPhase.SelectTarget || _currentCommand == null) return;
            
            _currentPhase.Value = TurnPhase.Execute;
            
            if (_currentCommand.CanExecute())
            {
                _currentCommand.Execute();
                _commandExecutedPublisher.Publish(new CommandExecutedEvent(_currentCommand));
            }
            
            _currentPhase.Value = TurnPhase.NextTurn;
        }
        
        private void CancelCommand()
        {
            switch (_currentPhase.Value)
            {
                case TurnPhase.SelectTarget:
                    _selectedTarget = null;
                    _currentCommand = null;
                    _targetSelector.ClearValidTargets();
                    _currentPhase.Value = TurnPhase.SelectSkill;
                    break;
                case TurnPhase.SelectSkill:
                    break;
            }
        }
        
        private void OnPhaseChanged(TurnPhase phase)
        {
            switch (phase)
            {
                case TurnPhase.Idle:
                    _selectedSkill = null;
                    _selectedTarget = null;
                    _currentCommand = null;
                    _targetSelector.ClearValidTargets();
                    break;
                case TurnPhase.NextTurn:
                    _currentPhase.Value = TurnPhase.Idle;
                    break;
            }
        }
    }
}