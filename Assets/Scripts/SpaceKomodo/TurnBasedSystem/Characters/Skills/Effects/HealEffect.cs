using System.Collections.Generic;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class HealEffect : BaseSkillEffect
    {
        public override EffectType Type => EffectType.Heal;
        
        public override void Execute(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel)
        {
            var healModel = (HealEffectModel)effectModel;
            var healAmount = healModel.Amount;
            var isCritical = IsCriticalHeal(healModel.CriticalChance);
            
            if (isCritical)
            {
                healAmount = Mathf.RoundToInt(healAmount * healModel.CriticalMultiplier);
            }
            
            var currentHealth = primaryTarget.CurrentHealth.Value;
            var maxHealth = primaryTarget.CurrentMaxHealth.Value;
            
            primaryTarget.CurrentHealth.Value = Mathf.Min(currentHealth + healAmount, maxHealth);
        }
        
        public override Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            var healModel = (HealEffectModel)effectModel;
            
            var normalHeal = healModel.Amount;
            var criticalHeal = Mathf.RoundToInt(healModel.Amount * healModel.CriticalMultiplier);
            
            result["MinHeal"] = normalHeal;
            result["MaxHeal"] = criticalHeal;
            result["CriticalChance"] = healModel.CriticalChance;
            
            return result;
        }
        
        private bool IsCriticalHeal(float chance)
        {
            return Random.value < chance;
        }
    }
}