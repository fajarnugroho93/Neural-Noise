using System.Collections.Generic;
using System.Linq;
using MessagePipe;
using ObservableCollections;
using R3;
using SpaceKomodo.Extensions;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Commands;
using SpaceKomodo.TurnBasedSystem.Events;
using SpaceKomodo.TurnBasedSystem.Maps;
using UnityEngine;
using VContainer;
using Random = System.Random;

namespace SpaceKomodo.TurnBasedSystem.Core
{
    public class TurnBasedModel : MonoBehaviour
    {
        [Inject] private readonly IPublisher<CurrentTurnCharacterSelectedEvent> currentTurnCharacterSelectedPublisher;
        [Inject] private readonly IEffectExecutor _effectExecutor;
        
        public CharacterScriptableObject[] heroes;
        public CharacterScriptableObject[] enemies;
        
        public readonly ReactiveProperty<int> CurrentRound = new(0);
        public readonly ReactiveProperty<int> CurrentTurn = new(0);

        public readonly ObservableList<CharacterModel> models = new();
        public readonly List<MapCharacterModel> heroMapModels = new();
        public readonly List<MapCharacterModel> enemyMapModels = new();
        private readonly List<int> turnSpeeds = new();
        
        private readonly Subject<Unit> _turnOrderChanged = new();
        private List<CharacterModel> _sortedModels;
        public Observable<Unit> TurnOrderChanged => _turnOrderChanged;

        public MapModel MapModel;
        
        public readonly ReactiveProperty<TurnPhase> CurrentPhase = new(TurnPhase.Idle);
        public CharacterModel CurrentCharacter { get; private set; }
        public SkillModel SelectedSkill { get; private set; }
        public CharacterModel SelectedTarget { get; private set; }
        public TurnCommand CurrentCommand { get; private set; }
        
        private readonly List<CharacterModel> _validTargets = new();
        public IReadOnlyList<CharacterModel> ValidTargets => _validTargets;

        public void SetupBattle()
        {
            MapModel = new MapModel(2, 4);
            
            CreateCharacterModels(MapGrid.HeroGrid, heroes, heroMapModels);
            CreateCharacterModels(MapGrid.EnemyGrid, enemies, enemyMapModels);

            void CreateCharacterModels(
                MapGrid mapGrid,
                IEnumerable<CharacterScriptableObject> characterScriptableObjects,
                ICollection<MapCharacterModel> modelList)
            {
                foreach (var characterScriptableObject in characterScriptableObjects)
                {
                    var newModel = new CharacterModel(characterScriptableObject.CharacterModel);
                    models.Add(newModel);
                    modelList.Add(MapModel.AddModel(mapGrid, newModel));
                    turnSpeeds.Add(turnSpeeds.Count);
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
            
            turnSpeeds.Shuffle();
            
            for (var ii = 0; ii < models.Count; ++ii)
            {
                models[turnSpeeds[ii]].SetTurnSpeed(ii + 1);
            }

            RecalculateTurnOrder();
            NextTurn();
        }
        
        private void RecalculateTurnOrder()
        {
            var random = new Random();
            
            _sortedModels = models
                .OrderByDescending(characterModel => characterModel.CurrentSpeed.Value)
                .ThenBy(_ => random.Next())
                .ToList();
                
            for (var ii = 0; ii < _sortedModels.Count; ii++)
            {
                _sortedModels[ii].TurnOrder.Value = ii;
            }
            
            _turnOrderChanged.OnNext(Unit.Default);
        }

        public bool HasNextTurn()
        {
            return CurrentTurn.Value < models.Count;
        }

        public void NextTurn()
        {
            ++CurrentTurn.Value;

            for (var ii = 0; ii < _sortedModels.Count; ++ii)
            {
                var currentCharacterModel = _sortedModels[ii];
                var isCurrentCharacterTurn = CurrentTurn.Value == ii + 1;

                if (isCurrentCharacterTurn)
                {
                    currentTurnCharacterSelectedPublisher.Publish(new CurrentTurnCharacterSelectedEvent(currentCharacterModel));
                }
                
                _sortedModels[ii].SetIsCurrentTurn(isCurrentCharacterTurn);
            }
            
            ResetTurnState();
        }
        
        public void SetCurrentCharacter(CharacterModel character)
        {
            CurrentCharacter = character;
            CurrentPhase.Value = TurnPhase.SelectSkill;
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
            
            SelectedTarget = target;
            CurrentPhase.Value = TurnPhase.Confirmation;
            
            CreateCommand();
        }
        
        private void CreateCommand()
        {
            if (CurrentCharacter == null || SelectedSkill == null || SelectedTarget == null) return;
            
            CurrentCommand = new SkillCommand(CurrentCharacter, SelectedSkill, SelectedTarget, _effectExecutor);
        }

        public bool IsValidTarget(CharacterModel target)
        {
            return _validTargets.Contains(target);
        }

        public void DetermineValidTargets()
        {
            ClearValidTargets();
            
            if (CurrentCharacter == null || SelectedSkill == null) return;
            
            switch (SelectedSkill.Target)
            {
                case SkillTarget.Self:
                    _validTargets.Add(CurrentCharacter);
                    break;
                    
                case SkillTarget.SingleAlly:
                    foreach (var model in models)
                    {
                        if (model != CurrentCharacter && model.IsHero() == CurrentCharacter.IsHero())
                        {
                            _validTargets.Add(model);
                        }
                    }
                    break;
                    
                case SkillTarget.SingleEnemy:
                    foreach (var model in models)
                    {
                        if (model.IsHero() != CurrentCharacter.IsHero())
                        {
                            _validTargets.Add(model);
                        }
                    }
                    break;
            }
        }
        
        public void ClearValidTargets()
        {
            _validTargets.Clear();
        }

        public void CancelSelectSkill()
        {
            SelectedSkill = null;
            SelectedTarget = null;
            CurrentPhase.Value = TurnPhase.Idle;
            ClearValidTargets();
        }

        public void CancelSelectTarget()
        {
            SelectedSkill = null;
            SelectedTarget = null;
            CurrentPhase.Value = TurnPhase.Idle;
            ClearValidTargets();
        }

        public void CancelConfirmation()
        {
            SelectedTarget = null;
            CurrentPhase.Value = TurnPhase.SelectTarget;
        }

        public void ExecuteCurrentCommand()
        {
            if (CurrentCommand == null || !CurrentCommand.CanExecute()) return;
            
            CurrentPhase.Value = TurnPhase.Execute;
            CurrentCommand.Execute();
            
            TransitionToNextTurn();
        }
        
        private void TransitionToNextTurn()
        {
            if (HasNextTurn())
            {
                NextTurn();
            }
            else
            {
                NextRound();
            }
        }

        public void ResetTurnState()
        {
            SelectedSkill = null;
            SelectedTarget = null;
            CurrentCommand = null;
            ClearValidTargets();
            CurrentPhase.Value = TurnPhase.Idle;
        }
    }
}