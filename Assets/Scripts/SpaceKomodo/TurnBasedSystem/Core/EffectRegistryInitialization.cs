using System;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models;
using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Core
{
    public static class EffectRegistryInitialization
    {
        public static void InitializeFromScriptableObjects(
            EffectRegistriesScriptableObject registries,
            DamageCalculator damageCalculator,
            StatusEffectManager statusEffectManager,
            ResourceManager resourceManager)
        {
            RegisterBasicEffects(registries.BasicEffects, damageCalculator);
            RegisterStatusEffects(registries.StatusEffects, statusEffectManager);
            RegisterResourceEffects(registries.ResourceEffects, resourceManager);
        }

        private static void RegisterBasicEffects(
            EffectRegistryScriptableObject[] basicEffects, 
            DamageCalculator damageCalculator)
        {
            foreach (var effectRegistry in basicEffects)
            {
                if (effectRegistry.EffectType == EffectType.Damage)
                {
                    var behavior = new DamageBehavior(damageCalculator);
                    RegisterEffect(effectRegistry, typeof(DamageEffectModel), () => new DamageEffectModel(), behavior);
                }
                else if (effectRegistry.EffectType == EffectType.Heal)
                {
                    var behavior = new HealBehavior();
                    RegisterEffect(effectRegistry, typeof(HealEffectModel), () => new HealEffectModel(), behavior);
                }
                else if (effectRegistry.EffectType == EffectType.Shield)
                {
                    var behavior = new ShieldBehavior();
                    RegisterEffect(effectRegistry, typeof(ShieldEffectModel), () => new ShieldEffectModel(), behavior);
                }
                else
                {
                    RegisterBasicEffectDynamic(effectRegistry, damageCalculator);
                }
            }
        }

        private static void RegisterBasicEffectDynamic(
            EffectRegistryScriptableObject effectRegistry, 
            DamageCalculator damageCalculator)
        {
            if (string.IsNullOrEmpty(effectRegistry.ImplementationClassName))
                return;

            Type behaviorType = Type.GetType($"SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.{effectRegistry.ImplementationClassName}");
            if (behaviorType == null)
                return;

            var modelType = Type.GetType($"SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.{effectRegistry.GetModelClassName()}");
            if (modelType == null)
                return;

            IEffectBehavior behavior = null;
            
            var constructor = behaviorType.GetConstructor(new[] { typeof(DamageCalculator) });
            if (constructor != null)
            {
                behavior = (IEffectBehavior)constructor.Invoke(new object[] { damageCalculator });
            }
            else
            {
                constructor = behaviorType.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    behavior = (IEffectBehavior)constructor.Invoke(null);
                }
            }

            if (behavior != null)
            {
                EffectTypeRegistry.RegisterEffectType(
                    effectRegistry.EffectType,
                    effectRegistry.Category,
                    modelType,
                    () => (IEffectModel)Activator.CreateInstance(modelType),
                    behavior
                );
            }
        }

        private static void RegisterStatusEffects(
            EffectRegistryScriptableObject[] statusEffects, 
            StatusEffectManager statusEffectManager)
        {
            foreach (var effectRegistry in statusEffects)
            {
                var statusBehavior = new StatusBehavior(statusEffectManager);
                
                if (effectRegistry.EffectType == EffectType.Poison)
                {
                    RegisterEffect(effectRegistry, typeof(PoisonEffectModel), () => new PoisonEffectModel(), statusBehavior);
                }
                else if (effectRegistry.EffectType == EffectType.Burn)
                {
                    RegisterEffect(effectRegistry, typeof(BurnEffectModel), () => new BurnEffectModel(), statusBehavior);
                }
                else if (effectRegistry.EffectType == EffectType.Stun)
                {
                    RegisterEffect(effectRegistry, typeof(StunEffectModel), () => new StunEffectModel(), statusBehavior);
                }
                else
                {
                    RegisterStatusEffectDynamic(effectRegistry, statusBehavior);
                }
            }
        }

        private static void RegisterStatusEffectDynamic(
            EffectRegistryScriptableObject effectRegistry, 
            StatusBehavior statusBehavior)
        {
            var modelType = Type.GetType($"SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.{effectRegistry.GetModelClassName()}");
            if (modelType == null)
                return;

            EffectTypeRegistry.RegisterEffectType(
                effectRegistry.EffectType,
                effectRegistry.Category,
                modelType,
                () => (IEffectModel)Activator.CreateInstance(modelType),
                statusBehavior
            );
        }

        private static void RegisterResourceEffects(
            EffectRegistryScriptableObject[] resourceEffects, 
            ResourceManager resourceManager)
        {
            foreach (var effectRegistry in resourceEffects)
            {
                var resourceBehavior = new ResourceBehavior(resourceManager);
                
                if (effectRegistry.EffectType == EffectType.Energy)
                {
                    RegisterEffect(effectRegistry, typeof(EnergyEffectModel), () => new EnergyEffectModel(), resourceBehavior);
                }
                else if (effectRegistry.EffectType == EffectType.Rage)
                {
                    RegisterEffect(effectRegistry, typeof(RageEffectModel), () => new RageEffectModel(), resourceBehavior);
                }
                else
                {
                    RegisterResourceEffectDynamic(effectRegistry, resourceBehavior);
                }
            }
        }

        private static void RegisterResourceEffectDynamic(
            EffectRegistryScriptableObject effectRegistry, 
            ResourceBehavior resourceBehavior)
        {
            var modelType = Type.GetType($"SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.{effectRegistry.GetModelClassName()}");
            if (modelType == null)
                return;

            EffectTypeRegistry.RegisterEffectType(
                effectRegistry.EffectType,
                effectRegistry.Category,
                modelType,
                () => (IEffectModel)Activator.CreateInstance(modelType),
                resourceBehavior
            );
        }

        private static void RegisterEffect(
            EffectRegistryScriptableObject effectRegistry,
            Type modelType,
            Func<IEffectModel> modelFactory,
            IEffectBehavior behavior)
        {
            EffectTypeRegistry.RegisterEffectType(
                effectRegistry.EffectType,
                effectRegistry.Category,
                modelType,
                modelFactory,
                behavior
            );
        }
    }
}