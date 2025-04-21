using System.Collections.Generic;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public class DamageEffect : BaseSkillEffect
    {
        private readonly DamageCalculator _damageCalculator;
        
        public DamageEffect(DamageCalculator damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }
        
        public override EffectType Type => EffectType.Damage;
        
        public override void Execute(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters)
        {
            var damageAmount = parameters.GetInt("Amount", 0);
            var isCritical = _damageCalculator.CalculateCritical(parameters.GetFloat("CriticalChance", 0), parameters.GetFloat("CriticalMultiplier", 1.5f));
            var damageType = (DamageType)parameters.GetInt("DamageType", (int)DamageType.Normal);
            
            var finalDamage = _damageCalculator.CalculateDamage(source, primaryTarget, damageAmount, isCritical, damageType);
            
            _damageCalculator.ApplyDamage(primaryTarget, finalDamage, damageType);
        }
        
        public override Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters)
        {
            var result = new Dictionary<string, object>();
            
            var damageAmount = parameters.GetInt("Amount", 0);
            var criticalChance = parameters.GetFloat("CriticalChance", 0);
            var criticalMultiplier = parameters.GetFloat("CriticalMultiplier", 1.5f);
            var damageType = (DamageType)parameters.GetInt("DamageType", (int)DamageType.Normal);
            
            var normalDamage = _damageCalculator.CalculateDamage(source, primaryTarget, damageAmount, false, damageType);
            var criticalDamage = _damageCalculator.CalculateDamage(source, primaryTarget, damageAmount, true, damageType);
            
            result["MinDamage"] = normalDamage;
            result["MaxDamage"] = criticalDamage;
            result["CriticalChance"] = criticalChance;
            result["DamageType"] = damageType;
            
            return result;
        }
    }
    
    public enum DamageType
    {
        Normal,
        Poison,
        Burn
    }
}