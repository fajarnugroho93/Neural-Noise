using System.Collections.Generic;
using System.Linq;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Core;
using SpaceKomodo.TurnBasedSystem.Maps;
using SpaceKomodo.TurnBasedSystem.Views;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public class TargetSelector : ITargetSelector
    {
        private readonly TurnBasedModel _turnModel;
        private readonly ITargetIndicatorFactory _targetIndicatorFactory;
        
        private readonly List<CharacterModel> _validTargets = new();
        private readonly Dictionary<CharacterModel, TargetIndicatorView> _targetIndicators = new();
        
        private CharacterModel _currentlySelectedTarget;
        
        public void SetSelectedTarget(CharacterModel target)
        {
            if (_currentlySelectedTarget != null && _targetIndicators.TryGetValue(_currentlySelectedTarget, out var oldIndicator))
            {
                oldIndicator.SetSelectedState(false);
            }
    
            _currentlySelectedTarget = target;
    
            if (target != null && _targetIndicators.TryGetValue(target, out var newIndicator))
            {
                newIndicator.SetSelectedState(true);
            }
        }

        public CharacterModel GetSelectedTarget()
        {
            return _currentlySelectedTarget;
        }
        
        public TargetSelector(
            TurnBasedModel turnModel,
            ITargetIndicatorFactory targetIndicatorFactory)
        {
            _turnModel = turnModel;
            _targetIndicatorFactory = targetIndicatorFactory;
        }
        
        public void SetValidTargets(CharacterModel source, SkillModel skill)
        {
            ClearValidTargets();
            
            switch (skill.Target)
            {
                case SkillTarget.Self:
                    AddValidTarget(source);
                    break;
                    
                case SkillTarget.SingleAlly:
                    foreach (var model in _turnModel.models)
                    {
                        if (model != source && model.IsHero() == source.IsHero())
                        {
                            AddValidTarget(model);
                        }
                    }
                    break;
                    
                case SkillTarget.SingleEnemy:
                    foreach (var model in _turnModel.models)
                    {
                        if (model.IsHero() != source.IsHero())
                        {
                            AddValidTarget(model);
                        }
                    }
                    break;
            }
        }
        
        private void AddValidTarget(CharacterModel target)
        {
            _validTargets.Add(target);
            
            if (_turnModel.MapModel != null)
            {
                var mapModel = FindMapCharacterModel(target);
                if (mapModel != null)
                {
                    var indicator = _targetIndicatorFactory.Create(mapModel);
                    _targetIndicators[target] = indicator;
                }
            }
        }
        
        private MapCharacterModel FindMapCharacterModel(CharacterModel character)
        {
            foreach (var mapModel in _turnModel.heroMapModels.Concat(_turnModel.enemyMapModels))
            {
                if (mapModel.CharacterModel == character)
                {
                    return mapModel;
                }
            }
            return null;
        }
        
        public bool IsValidTarget(CharacterModel target)
        {
            return _validTargets.Contains(target);
        }
        
        public List<CharacterModel> GetValidTargets()
        {
            return _validTargets;
        }
        
        public void ClearValidTargets()
        {
            _validTargets.Clear();
            _currentlySelectedTarget = null;
            
            foreach (var indicator in _targetIndicators.Values)
            {
                Object.Destroy(indicator.gameObject);
            }
            _targetIndicators.Clear();
        }
    }
}