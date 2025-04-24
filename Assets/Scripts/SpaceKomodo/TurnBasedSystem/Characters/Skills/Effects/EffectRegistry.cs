using System;
using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class EffectRegistry
    {
        private readonly Dictionary<EffectType, Func<IEffectModel>> _modelFactories = new Dictionary<EffectType, Func<IEffectModel>>();
        private readonly Dictionary<EffectType, IEffectBehavior> _behaviors = new Dictionary<EffectType, IEffectBehavior>();

        public EffectRegistry(
            DamageCalculator damageCalculator,
            StatusEffectManager statusEffectManager,
            ResourceManager resourceManager)
        {
            RegisterModelFactories();
            RegisterBehaviors(damageCalculator, statusEffectManager, resourceManager);
        }

        private void RegisterModelFactories()
        {
            RegisterModelFactory(EffectType.Damage, () => new DamageEffectModel { Amount = 10, CriticalChance = 0.1f, CriticalMultiplier = 1.5f });
            RegisterModelFactory(EffectType.Heal, () => new HealEffectModel { Amount = 15, CriticalChance = 0.1f, CriticalMultiplier = 1.5f });
            RegisterModelFactory(EffectType.Shield, () => new ShieldEffectModel { Amount = 10, Duration = 3 });
            RegisterModelFactory(EffectType.Poison, () => new PoisonEffectModel { Amount = 5, Duration = 3 });
            RegisterModelFactory(EffectType.Burn, () => new BurnEffectModel { Amount = 5, Duration = 3  });
            RegisterModelFactory(EffectType.Stun, () => new StunEffectModel { Amount = 1, Duration = 1  });
            RegisterModelFactory(EffectType.Energy, () => new EnergyEffectModel { Amount = 10 });
            RegisterModelFactory(EffectType.Rage, () => new RageEffectModel { Amount = 5 });
        }

        private void RegisterBehaviors(
            DamageCalculator damageCalculator,
            StatusEffectManager statusEffectManager,
            ResourceManager resourceManager)
        {
            var damageBehavior = new DamageBehavior(damageCalculator);
            var healBehavior = new HealBehavior();
            var shieldBehavior = new ShieldBehavior();
            var statusBehavior = new StatusBehavior(statusEffectManager);
            var resourceBehavior = new ResourceBehavior(resourceManager);

            RegisterBehavior(EffectType.Damage, damageBehavior);
            RegisterBehavior(EffectType.Heal, healBehavior);
            RegisterBehavior(EffectType.Shield, shieldBehavior);
            
            RegisterBehavior(EffectType.Poison, damageBehavior);
            RegisterBehavior(EffectType.Burn, damageBehavior);
            RegisterBehavior(EffectType.Stun, statusBehavior);
            RegisterBehavior(EffectType.Blind, statusBehavior);
            RegisterBehavior(EffectType.Silence, statusBehavior);
            RegisterBehavior(EffectType.Root, statusBehavior);
            RegisterBehavior(EffectType.Taunt, statusBehavior);
            
            RegisterBehavior(EffectType.Energy, resourceBehavior);
            RegisterBehavior(EffectType.Rage, resourceBehavior);
            RegisterBehavior(EffectType.Mana, resourceBehavior);
            RegisterBehavior(EffectType.Focus, resourceBehavior);
            RegisterBehavior(EffectType.Charge, resourceBehavior);
        }

        public void RegisterModelFactory(EffectType type, Func<IEffectModel> factory)
        {
            _modelFactories[type] = factory;
        }

        public void RegisterBehavior(EffectType type, IEffectBehavior behavior)
        {
            _behaviors[type] = behavior;
        }

        public IEffectModel CreateModel(EffectType type)
        {
            if (_modelFactories.TryGetValue(type, out var factory))
            {
                return factory();
            }

            throw new ArgumentException($"No model factory registered for effect type {type}");
        }

        public IEffectBehavior GetBehavior(EffectType type)
        {
            if (_behaviors.TryGetValue(type, out var behavior))
            {
                return behavior;
            }

            throw new ArgumentException($"No behavior registered for effect type {type}");
        }
    }
}