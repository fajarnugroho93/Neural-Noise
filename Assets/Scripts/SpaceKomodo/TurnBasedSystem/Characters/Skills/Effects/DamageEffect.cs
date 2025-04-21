using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class DamageEffect : BaseSkillEffect
    {
        private readonly DamageCalculator _damageCalculator;
        
        public DamageEffect(DamageCalculator damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }
        
        public override EffectType Type => EffectType.Damage;
        
        public override void Execute(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel)
        {
            var damageModel = (DamageEffectModel)effectModel;
            
            var isCritical = _damageCalculator.CalculateCritical(damageModel.CriticalChance, damageModel.CriticalMultiplier);
            var finalDamage = _damageCalculator.CalculateDamage(source, primaryTarget, damageModel.Amount, isCritical, damageModel.DamageType);
            
            _damageCalculator.ApplyDamage(primaryTarget, finalDamage, damageModel.DamageType);
        }
        
        public override Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            var damageModel = (DamageEffectModel)effectModel;
            
            var normalDamage = _damageCalculator.CalculateDamage(source, primaryTarget, damageModel.Amount, false, damageModel.DamageType);
            var criticalDamage = _damageCalculator.CalculateDamage(source, primaryTarget, damageModel.Amount, true, damageModel.DamageType);
            
            result["MinDamage"] = normalDamage;
            result["MaxDamage"] = criticalDamage;
            result["CriticalChance"] = damageModel.CriticalChance;
            result["DamageType"] = damageModel.DamageType;
            
            return result;
        }
    }
}