using System.Collections.Generic;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public class HealEffect : BaseSkillEffect
    {
        public override EffectType Type => EffectType.Heal;
        
        public override void Execute(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters)
        {
            var healAmount = parameters.GetInt("Amount", 0);
            var isCritical = IsCriticalHeal(parameters.GetFloat("CriticalChance", 0));
            
            if (isCritical)
            {
                healAmount = Mathf.RoundToInt(healAmount * parameters.GetFloat("CriticalMultiplier", 1.5f));
            }
            
            var currentHealth = primaryTarget.CurrentHealth.Value;
            var maxHealth = primaryTarget.CurrentMaxHealth.Value;
            
            primaryTarget.CurrentHealth.Value = Mathf.Min(currentHealth + healAmount, maxHealth);
        }
        
        public override Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters)
        {
            var result = new Dictionary<string, object>();
            
            var healAmount = parameters.GetInt("Amount", 0);
            var criticalChance = parameters.GetFloat("CriticalChance", 0);
            var criticalMultiplier = parameters.GetFloat("CriticalMultiplier", 1.5f);
            
            var normalHeal = healAmount;
            var criticalHeal = Mathf.RoundToInt(healAmount * criticalMultiplier);
            
            result["MinHeal"] = normalHeal;
            result["MaxHeal"] = criticalHeal;
            result["CriticalChance"] = criticalChance;
            
            return result;
        }
        
        private bool IsCriticalHeal(float chance)
        {
            return Random.value < chance;
        }
    }
}