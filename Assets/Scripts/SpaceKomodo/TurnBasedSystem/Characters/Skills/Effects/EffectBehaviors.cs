using System.Collections.Generic;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class DamageBehavior : IEffectBehavior
    {
        private readonly DamageCalculator _damageCalculator;

        public DamageBehavior(DamageCalculator damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }

        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is InstantEffectModel damageModel))
                return;

            var isCritical = _damageCalculator.CalculateCritical(damageModel.CriticalChance, damageModel.CriticalMultiplier);
            var finalDamage = _damageCalculator.CalculateDamage(source, target, damageModel.Amount, isCritical, GetDamageType(effectModel.Type));
            
            _damageCalculator.ApplyDamage(target, finalDamage, GetDamageType(effectModel.Type));
        }

        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is InstantEffectModel damageModel))
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
            switch (effectType)
            {
                case EffectType.Poison: return DamageType.Poison;
                case EffectType.Burn: return DamageType.Burn;
                default: return DamageType.Normal;
            }
        }
    }

    public class HealBehavior : IEffectBehavior
    {
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is InstantEffectModel healModel))
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
            
            if (!(effectModel is InstantEffectModel healModel))
                return result;

            var normalHeal = healModel.Amount;
            var criticalHeal = Mathf.RoundToInt(healModel.Amount * healModel.CriticalMultiplier);

            result["MinHeal"] = normalHeal;
            result["MaxHeal"] = criticalHeal;
            result["CriticalChance"] = healModel.CriticalChance;

            return result;
        }
    }

    public class ShieldBehavior : IEffectBehavior
    {
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is StatusEffectModel shieldModel))
                return;

            target.CurrentShield.Value += shieldModel.Amount;
        }

        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is StatusEffectModel shieldModel))
                return result;

            result["ShieldAmount"] = shieldModel.Amount;
            result["Duration"] = shieldModel.Duration;

            return result;
        }
    }

    public class StatusBehavior : IEffectBehavior
    {
        private readonly StatusEffectManager _statusEffectManager;
        
        public StatusBehavior(StatusEffectManager statusEffectManager)
        {
            _statusEffectManager = statusEffectManager;
        }
        
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is IStatusEffect statusModel))
                return;
                
            _statusEffectManager.ApplyStatus(
                target, 
                (int)effectModel.Type, 
                statusModel.Duration, 
                statusModel.Amount);
        }
        
        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is IStatusEffect statusModel))
                return result;
                
            result["StatusType"] = effectModel.Type;
            result["Duration"] = statusModel.Duration;
            result["Intensity"] = statusModel.Amount;
            
            return result;
        }
    }

    public class ResourceBehavior : IEffectBehavior
    {
        private readonly ResourceManager _resourceManager;
        
        public ResourceBehavior(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }
        
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is StatusEffectModel resourceModel))
                return;
            
            var amount = resourceModel.Amount;
            var resourceType = (int)effectModel.Type;
            
            if (amount > 0)
            {
                _resourceManager.AddResource(target, resourceType, amount);
            }
            else if (amount < 0)
            {
                _resourceManager.ConsumeResource(target, resourceType, -amount);
            }
        }
        
        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is StatusEffectModel resourceModel))
                return result;
            
            result["ResourceType"] = effectModel.Type;
            result["Amount"] = resourceModel.Amount;
            
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