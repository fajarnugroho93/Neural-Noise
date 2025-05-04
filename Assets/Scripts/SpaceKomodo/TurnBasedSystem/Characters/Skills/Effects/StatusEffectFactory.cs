using System;
using System.Collections.Generic;
using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class StatusEffectFactory
    {
        private readonly DamageCalculator _damageCalculator;
        private readonly Dictionary<EffectType, Type> _implementationTypes = new();
        
        public StatusEffectFactory(DamageCalculator damageCalculator, EffectRegistriesScriptableObject registries)
        {
            _damageCalculator = damageCalculator;
            
            if (registries != null)
            {
                foreach (var registry in registries.StatusEffects)
                {
                    var implementationClass = registry.BehaviorClassName;
                    if (string.IsNullOrEmpty(implementationClass))
                        continue;
                        
                    var type = Type.GetType($"{Constants.EffectsBehaviorsPath}.{implementationClass}");
                    if (type != null)
                    {
                        _implementationTypes[registry.EffectType] = type;
                    }
                }
            }
        }
        
        public IStatusEffectBehavior CreateImplementation(EffectType effectType)
        {
            if (_implementationTypes.TryGetValue(effectType, out var type))
            {
                var constructor = type.GetConstructor(new[] { typeof(EffectType), typeof(DamageCalculator) });
                if (constructor != null)
                {
                    return constructor.Invoke(new object[] { effectType, _damageCalculator }) as IStatusEffectBehavior;
                }
                
                constructor = type.GetConstructor(new[] { typeof(EffectType) });
                if (constructor != null)
                {
                    return constructor.Invoke(new object[] { effectType }) as IStatusEffectBehavior;
                }
            }

            return null;
        }
    }
}