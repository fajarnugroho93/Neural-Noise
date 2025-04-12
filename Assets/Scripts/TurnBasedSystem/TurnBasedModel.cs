using System;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using SpaceKomodo.Extensions;
using TurnBasedSystem.Characters;
using UnityEngine;
using Random = System.Random;

namespace TurnBasedSystem
{
    public class TurnBasedModel : MonoBehaviour
    {
        public CharacterScriptableObject[] heroes;
        public CharacterScriptableObject[] enemies;
        
        public ReactiveProperty<int> CurrentRound;
        public ReactiveProperty<int> CurrentTurn;

        public readonly ObservableList<CharacterModel> models = new();
        private readonly List<int> turnSpeeds = new();
        
        private readonly Subject<Unit> _turnOrderChanged = new();
        private List<CharacterModel> _sortedModels;
        public Observable<Unit> TurnOrderChanged => _turnOrderChanged;

        public void BeginBattle()
        {
            foreach (var hero in heroes)
            {
                models.Add(new CharacterModel(hero.CharacterModel));
                turnSpeeds.Add(turnSpeeds.Count);
            }
            
            foreach (var enemy in enemies)
            {
                models.Add(new CharacterModel(enemy.CharacterModel));
                turnSpeeds.Add(turnSpeeds.Count);
            }

            CurrentRound = new ReactiveProperty<int>(0);
            CurrentTurn = new ReactiveProperty<int>(0);
            
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
                _sortedModels[ii].SetIsCurrentTurn(CurrentTurn.Value == ii + 1);
            }
        }
    }
}