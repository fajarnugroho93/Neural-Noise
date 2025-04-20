using System.Collections.Generic;
using System.Linq;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Core;
using SpaceKomodo.TurnBasedSystem.Maps;
using SpaceKomodo.TurnBasedSystem.Views;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public class TargetIndicatorManager : ITargetIndicatorManager
    {
        private readonly TurnBasedModel _turnModel;
        private readonly ITargetIndicatorFactory _targetIndicatorFactory;
        
        private readonly Dictionary<CharacterModel, TargetIndicatorView> _targetIndicators = new();
        private CharacterModel _selectedTarget;
        
        public TargetIndicatorManager(
            TurnBasedModel turnModel,
            ITargetIndicatorFactory targetIndicatorFactory)
        {
            _turnModel = turnModel;
            _targetIndicatorFactory = targetIndicatorFactory;
        }
        
        public void UpdateTargetIndicators(IReadOnlyList<CharacterModel> validTargets)
        {
            ClearTargetIndicators();
            
            foreach (var target in validTargets)
            {
                var mapModel = FindMapCharacterModel(target);
                if (mapModel != null)
                {
                    var indicator = _targetIndicatorFactory.Create(mapModel);
                    _targetIndicators[target] = indicator;
                }
            }
        }
        
        public void SetSelectedTarget(CharacterModel target)
        {
            if (_selectedTarget != null && _targetIndicators.TryGetValue(_selectedTarget, out var oldIndicator))
            {
                oldIndicator.SetSelectedState(false);
            }
            
            _selectedTarget = target;
            
            if (target != null && _targetIndicators.TryGetValue(target, out var newIndicator))
            {
                newIndicator.SetSelectedState(true);
            }
        }
        
        public void ClearTargetIndicators()
        {
            _selectedTarget = null;
            
            foreach (var indicator in _targetIndicators.Values)
            {
                if (indicator != null)
                {
                    Object.Destroy(indicator.gameObject);
                }
            }
            
            _targetIndicators.Clear();
        }
        
        private MapCharacterModel FindMapCharacterModel(CharacterModel character)
        {
            return _turnModel.heroMapModels
                .Concat(_turnModel.enemyMapModels)
                .FirstOrDefault(mapModel => mapModel.CharacterModel == character);
        }
    }
}