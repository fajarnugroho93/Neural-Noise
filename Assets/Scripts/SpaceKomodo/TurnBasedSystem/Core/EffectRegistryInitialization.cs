using System;
using System.Collections.Generic;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors;
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
            IEnumerable<EffectRegistryScriptableObject> basicEffects, 
            DamageCalculator damageCalculator)
        {
            foreach (var effectRegistry in basicEffects)
            {
                RegisterBasicEffect(effectRegistry, damageCalculator);
            }
        }

        private static void RegisterBasicEffect(
            EffectRegistryScriptableObject effectRegistry, 
            DamageCalculator damageCalculator)
        {
            if (string.IsNullOrEmpty(effectRegistry.BehaviorClassName))
                return;

            var behaviorType = Type.GetType($"{Constants.EffectsBehaviorsPath}.{effectRegistry.BehaviorClassName}");
            if (behaviorType == null)
                return;

            var modelType = Type.GetType($"{Constants.EffectsModelsPath}.{effectRegistry.ModelClassName}");
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
            IEnumerable<EffectRegistryScriptableObject> statusEffects, 
            StatusEffectManager statusEffectManager)
        {
            foreach (var effectRegistry in statusEffects)
            {
                var statusBehavior = new StatusBehavior(statusEffectManager);

                RegisterStatusEffect(effectRegistry, statusBehavior);
            }
        }

        private static void RegisterStatusEffect(
            EffectRegistryScriptableObject effectRegistry, 
            IEffectBehavior statusBehavior)
        {
            var modelType = Type.GetType($"{Constants.EffectsModelsPath}.{effectRegistry.ModelClassName}");
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
            IEnumerable<EffectRegistryScriptableObject> resourceEffects, 
            ResourceManager resourceManager)
        {
            foreach (var effectRegistry in resourceEffects)
            {
                var resourceBehavior = new ResourceBehavior(resourceManager);
                
                RegisterResourceEffect(effectRegistry, resourceBehavior);
            }
        }

        private static void RegisterResourceEffect(
            EffectRegistryScriptableObject effectRegistry, 
            IEffectBehavior resourceBehavior)
        {
            var modelType = Type.GetType($"{Constants.EffectsModelsPath}.{effectRegistry.ModelClassName}");
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
    }
}