using System.Collections.Generic;
using System.Linq;
using MessagePipe;
using ObservableCollections;
using R3;
using SpaceKomodo.Extensions;
using SpaceKomodo.TurnBasedSystem.Characters;
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
        
        public CharacterScriptableObject[] heroes;
        public CharacterScriptableObject[] enemies;
        
        public ReactiveProperty<int> CurrentRound = new(0);
        public ReactiveProperty<int> CurrentTurn = new(0);

        public readonly ObservableList<CharacterModel> models = new();
        public readonly List<MapCharacterModel> heroMapModels = new();
        public readonly List<MapCharacterModel> enemyMapModels = new();
        private readonly List<int> turnSpeeds = new();
        
        private readonly Subject<Unit> _turnOrderChanged = new();
        private List<CharacterModel> _sortedModels;
        public Observable<Unit> TurnOrderChanged => _turnOrderChanged;

        public MapModel MapModel;
        
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
        }
    }
}