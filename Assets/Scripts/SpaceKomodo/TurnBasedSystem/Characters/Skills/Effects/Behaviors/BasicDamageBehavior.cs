using System.Collections.Generic;
using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors
{
    public class BasicDamageBehavior : IEffectBehavior
    {
        private readonly DamageCalculator _damageCalculator;

        public BasicDamageBehavior(DamageCalculator damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }

        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is IInstantEffect damageModel))
                return;

            var isCritical = _damageCalculator.CalculateCritical(damageModel.CriticalChance, damageModel.CriticalMultiplier);
            var finalDamage = _damageCalculator.CalculateDamage(source, target, damageModel.Amount, isCritical, GetDamageType(effectModel.Type));
            
            _damageCalculator.ApplyDamage(target, finalDamage, GetDamageType(effectModel.Type));
        }

        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is IInstantEffect damageModel))
                return result;

            var normalDamage = _damageCalculator.CalculateDamage(source, target, damageModel.Amount, false, GetDamageType(effectModel.Type));
            var criticalDamage = _damageCalculator.CalculateDamage(source, target, damageModel.Amount, true, GetDamageType(effectModel.Type));

            result["MinDamage"] = normalDamage;
            result["MaxDamage"] = criticalDamage;
            result["CriticalChance"] = damageModel.CriticalChance;
            result["DamageType"] = GetDamageType(effectModel.Type);

            return result;
        }

        private DamageType GetDamageType(EffectType effectType)
        {
            return effectType switch
            {
                EffectType.Poison => DamageType.Poison,
                EffectType.Burn => DamageType.Burn,
                _ => DamageType.Normal
            };
        }
    }
}