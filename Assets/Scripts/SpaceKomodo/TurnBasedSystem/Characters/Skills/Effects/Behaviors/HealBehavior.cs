using System.Collections.Generic;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors
{
    public class HealBehavior : IEffectBehavior
    {
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is IInstantEffect healModel))
                return;

            var healAmount = healModel.Amount;
            var isCritical = Random.value < healModel.CriticalChance;

            if (isCritical)
            {
                healAmount = Mathf.RoundToInt(healAmount * healModel.CriticalMultiplier);
            }

            var currentHealth = target.CurrentHealth.Value;
            var maxHealth = target.CurrentMaxHealth.Value;

            target.CurrentHealth.Value = Mathf.Min(currentHealth + healAmount, maxHealth);
        }

        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is IInstantEffect healModel))
                return result;

            var normalHeal = healModel.Amount;
            var criticalHeal = Mathf.RoundToInt(healModel.Amount * healModel.CriticalMultiplier);

            result["MinHeal"] = normalHeal;
            result["MaxHeal"] = criticalHeal;
            result["CriticalChance"] = healModel.CriticalChance;

            return result;
        }
    }
}