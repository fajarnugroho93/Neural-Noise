using System;
using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class EffectFactory
    {
        private readonly Dictionary<EffectType, Func<ISkillEffect>> _effectFactories = new Dictionary<EffectType, Func<ISkillEffect>>();
        
        public EffectFactory(DamageCalculator damageCalculator, ResourceManager resourceManager, StatusEffectManager statusEffectManager)
        {
            RegisterEffect(EffectType.Damage, () => new DamageEffect(damageCalculator));
            RegisterEffect(EffectType.Heal, () => new HealEffect());
            RegisterEffect(EffectType.Shield, () => new ShieldEffect());
            RegisterEffect(EffectType.Resource, () => new ResourceEffect(resourceManager));
            RegisterEffect(EffectType.Status, () => new StatusEffect(statusEffectManager));
        }
        
        public void RegisterEffect(EffectType type, Func<ISkillEffect> factory)
        {
            _effectFactories[type] = factory;
        }
        
        public ISkillEffect CreateEffect(EffectType type)
        {
            if (_effectFactories.TryGetValue(type, out var factory))
            {
                return factory();
            }
            
            throw new ArgumentException($"No factory registered for effect type {type}");
        }
    }
}