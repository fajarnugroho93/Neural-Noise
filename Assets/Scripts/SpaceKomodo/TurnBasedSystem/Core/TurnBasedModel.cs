using System.Collections.Generic;
using System.Linq;
using MessagePipe;
using ObservableCollections;
using R3;
using SpaceKomodo.Extensions;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Commands;
using SpaceKomodo.TurnBasedSystem.Dice;
using SpaceKomodo.TurnBasedSystem.Effects;
using SpaceKomodo.TurnBasedSystem.Events;
using SpaceKomodo.TurnBasedSystem.Maps;
using UnityEngine;
using VContainer;
using DisposableBag = R3.DisposableBag;
using Random = UnityEngine.Random;

namespace SpaceKomodo.TurnBasedSystem.Core
{
    public class TurnBasedModel : MonoBehaviour
    {
        private const bool IsUsingRandomSpeedBonus = false;
        
        [Inject] private readonly IPublisher<CurrentTurnCharacterSelectedEvent> currentTurnCharacterSelectedPublisher;
        [Inject] private readonly SkillExecutor _skillExecutor;
        [Inject] private readonly BattleModel _battleModel;
        [Inject] private readonly StatusEffectManager _statusEffectManager;
        
        public CharacterScriptableObject[] heroes;
        public CharacterScriptableObject[] enemies;
        public int diceAmount;
        
        public readonly ReactiveProperty<int> CurrentRound = new(0);
        public readonly ReactiveProperty<int> CurrentTurn = new(0);

        public readonly ObservableList<CharacterModel> characterModels = new();
        public readonly List<MapCharacterModel> heroMapModels = new();
        public readonly List<MapCharacterModel> enemyMapModels = new();
        private readonly List<int> turnSpeeds = new();

        public readonly ObservableList<DiceModel> diceModels = new();
        
        private readonly Subject<Unit> _turnOrderChanged = new();
        private List<CharacterModel> _sortedModels;
        public Observable<Unit> TurnOrderChanged => _turnOrderChanged;

        public MapModel MapModel;
        
        public readonly ReactiveProperty<TurnPhase> CurrentPhase = new(TurnPhase.Idle);
        private CharacterModel CurrentCharacter { get; set; }
        public DiceModel SelectedDice { get; private set; }
        public SkillModel SelectedSkill { get; private set; }
        public CharacterModel SelectedTarget { get; private set; }
        public TurnCommand CurrentCommand { get; private set; }
        
        private readonly List<SkillModel> _validSkills = new();
        public IReadOnlyList<SkillModel> ValidSkills => _validSkills;

        private DisposableBag _disposableBag;

        public TurnBasedModel()
        {
            _disposableBag = new DisposableBag();
        }

        public void SetupBattle()
        {
            MapModel = new MapModel(2, 4);
            
            CreateCharacterModels(MapGrid.HeroGrid, heroes, heroMapModels);
            CreateCharacterModels(MapGrid.EnemyGrid, enemies, enemyMapModels);

            _battleModel.RegisterAllCharacters(this);

            CreateDiceModels();
            
            void CreateCharacterModels(
                MapGrid mapGrid,
                IEnumerable<CharacterScriptableObject> characterScriptableObjects,
                ICollection<MapCharacterModel> modelList)
            {
                foreach (var characterScriptableObject in characterScriptableObjects)
                {
                    var newModel = new CharacterModel(characterScriptableObject.CharacterModel);
                    characterModels.Add(newModel);
                    modelList.Add(MapModel.AddModel(mapGrid, newModel));
                    turnSpeeds.Add(turnSpeeds.Count);
                }
            }

            void CreateDiceModels()
            {
                for (var i = 0; i < diceAmount; ++i)
                {
                    var newModel = new DiceModel(Random.Range(1, 7), _disposableBag);
                    diceModels.Add(newModel);
                }
            }
        }
        
        public void BeginBattle()
        {
            NextRound();
        }

        public void NextRound()
        {
            ++CurrentRound.Value;
            CurrentTurn.Value = 0;

            RecalculateRandomSpeedBonus();

            RecalculateTurnOrder();
            
            _statusEffectManager.ProcessRound();
            
            NextTurn();
        }

        private void RecalculateRandomSpeedBonus()
        {
            if (!IsUsingRandomSpeedBonus)
            {
                return;
            }
            
            turnSpeeds.Shuffle();
            
            for (var ii = 0; ii < characterModels.Count; ++ii)
            {
                characterModels[turnSpeeds[ii]].TurnSpeed.Value = (ii + 1);
            }
        }

        private void RecalculateTurnOrder()
        {
            _sortedModels = characterModels
                .OrderByDescending(characterModel => characterModel.CurrentSpeed.Value)
                .ThenBy(_ => Random.Range(int.MinValue, int.MaxValue))
                .ToList();
                
            for (var ii = 0; ii < _sortedModels.Count; ii++)
            {
                _sortedModels[ii].TurnOrder.Value = ii;
            }
            
            _turnOrderChanged.OnNext(Unit.Default);
        }

        public bool HasNextTurn()
        {
            return CurrentTurn.Value < characterModels.Count;
        }

        public void NextTurn()
        {
            ++CurrentTurn.Value;
            
            ResetTurnState();

            for (var ii = 0; ii < _sortedModels.Count; ++ii)
            {
                var currentCharacterModel = _sortedModels[ii];
                var isCurrentCharacterTurn = CurrentTurn.Value == ii + 1;

                if (isCurrentCharacterTurn)
                {
                    currentTurnCharacterSelectedPublisher.Publish(new CurrentTurnCharacterSelectedEvent(currentCharacterModel));
                }

                _sortedModels[ii].IsCurrentTurn.Value = isCurrentCharacterTurn;
            }
        }
        
        public void SetCurrentCharacter(CharacterModel character)
        {
            CurrentCharacter = character;
            CurrentPhase.Value = TurnPhase.SelectSkill;
        }

        public void SetSelectedDice(DiceModel dice)
        {
            SelectedDice = dice;
            SelectedSkill = null;
            SelectedTarget = null;
            CurrentCommand = null;
            CurrentPhase.Value = TurnPhase.SelectTarget;
            
            // DetermineValidSkills();
        }

        public void SetSelectedSkill(SkillModel skill)
        {
            SelectedSkill = skill;
            SelectedTarget = null;
            CurrentCommand = null;
            CurrentPhase.Value = TurnPhase.SelectTarget;
            
            DetermineValidTargets();
        }

        public void SetSelectedTarget(CharacterModel target)
        {
            if (!IsValidTarget(target)) return;

            target.IsTargeted.Value = true;
            SelectedTarget = target;
            CurrentPhase.Value = TurnPhase.Confirmation;
            
            CreateCommand();
        }
        
        private void CreateCommand()
        {
            if (CurrentCharacter == null || SelectedSkill == null || SelectedTarget == null) return;
            
            CurrentCommand = new SkillCommand(CurrentCharacter, SelectedSkill, SelectedTarget, _skillExecutor);
        }

        public bool IsValidTarget(CharacterModel target)
        {
            return target.IsTargetable.Value;
        }

        private void DetermineValidTargets()
        {
            ClearValidTargets();
            
            if (CurrentCharacter == null || SelectedSkill == null) return;
            
            switch (SelectedSkill.Target)
            {
                case SkillTarget.Self:
                    CurrentCharacter.IsTargetable.Value = true;
                    break;
                    
                case SkillTarget.SingleAlly:
                    foreach (var model in characterModels)
                    {
                        if (model != CurrentCharacter && model.IsHero() == CurrentCharacter.IsHero())
                        {
                            model.IsTargetable.Value = true;
                        }
                    }
                    break;
                    
                case SkillTarget.SingleEnemy:
                    foreach (var model in characterModels)
                    {
                        if (model.IsHero() != CurrentCharacter.IsHero())
                        {
                            model.IsTargetable.Value = true;
                        }
                    }
                    break;
            }
        }
        
        public void ClearValidTargets()
        {
            foreach (var characterModel in characterModels)
            {
                characterModel.ResetTarget();
            }
            SelectedTarget = null;
        }
        
        public void ClearSelectedTargets()
        {
            SelectedTarget.IsTargeted.Value = false;
            SelectedTarget = null;
        }

        public void CancelSelectDice()
        {
            SelectedDice = null;
            SelectedSkill = null;
            CurrentPhase.Value = TurnPhase.Idle;
            ClearValidTargets();
        }

        public void CancelSelectSkill()
        {
            SelectedSkill = null;
            CurrentPhase.Value = TurnPhase.Idle;
            ClearValidTargets();
        }

        public void CancelSelectTarget()
        {
            SelectedSkill = null;
            CurrentPhase.Value = TurnPhase.Idle;
            ClearValidTargets();
        }

        public void CancelConfirmation()
        {
            ClearSelectedTargets();
            CurrentPhase.Value = TurnPhase.SelectTarget;
        }

        public void ExecuteCurrentCommand()
        {
            if (CurrentCommand == null || !CurrentCommand.CanExecute()) return;
            
            CurrentCommand.Execute();
            CurrentPhase.Value = TurnPhase.Execute;
        }

        private void ClearActiveCharacter()
        {
            foreach (var characterModel in characterModels)
            {
                characterModel.IsCurrentTurn.Value = false;
            }
        }

        private void ResetTurnState()
        {
            ClearActiveCharacter();
            ClearValidTargets();
            
            SelectedSkill = null;
            CurrentCommand = null;
            CurrentPhase.Value = TurnPhase.Idle;
        }
    }
}