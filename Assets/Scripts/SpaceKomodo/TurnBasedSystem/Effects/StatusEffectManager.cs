using System.Collections.Generic;
using System.Linq;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;

namespace SpaceKomodo.TurnBasedSystem.Effects
{
    public class StatusEffectManager
    {
        private readonly Dictionary<CharacterModel, List<ActiveStatusEffect>> _activeEffects = new();
        private readonly Dictionary<EffectType, IStatusEffectImplementation> _statusImplementations = new();

        public StatusEffectManager()
        {
            
        }
        
        public StatusEffectManager(IEnumerable<IStatusEffectImplementation> implementations)
        {
            foreach (var implementation in implementations)
            {
                if (implementation is BaseStatusImplementation statusImplementation)
                {
                    RegisterStatusImplementation(statusImplementation.EffectType, implementation);
                }
            }
        }
        
        public void RegisterStatusImplementation(EffectType effectType, IStatusEffectImplementation implementation)
        {
            _statusImplementations[effectType] = implementation;
        }
        
        public void ApplyStatus(CharacterModel target, EffectType effectType, int duration, int intensity)
        {
            if (!_activeEffects.TryGetValue(target, out var effectsList))
            {
                effectsList = new List<ActiveStatusEffect>();
                _activeEffects[target] = effectsList;
            }
            
            var existingEffect = effectsList.FirstOrDefault(e => e.EffectType == effectType);
            
            if (existingEffect != null)
            {
                existingEffect.Duration = duration;
                existingEffect.Intensity += intensity;
            }
            else
            {
                effectsList.Add(new ActiveStatusEffect
                {
                    EffectType = effectType,
                    Duration = duration,
                    Intensity = intensity
                });
                
                if (_statusImplementations.TryGetValue(effectType, out var implementation))
                {
                    implementation.OnApplied(target, intensity);
                }
            }
        }
        
        public void ProcessRound()
        {
            foreach (var characterEffects in _activeEffects.ToList())
            {
                var character = characterEffects.Key;
                var effects = characterEffects.Value;
                
                foreach (var effect in effects.ToList())
                {
                    if (_statusImplementations.TryGetValue(effect.EffectType, out var implementation))
                    {
                        implementation.OnRoundStart(character, effect.Intensity);
                    }
                    
                    effect.Duration--;
                    
                    if (effect.Duration <= 0)
                    {
                        if (_statusImplementations.TryGetValue(effect.EffectType, out implementation))
                        {
                            implementation.OnRemoved(character, effect.Intensity);
                        }
                        
                        effects.Remove(effect);
                    }
                }
                
                if (effects.Count == 0)
                {
                    _activeEffects.Remove(character);
                }
            }
        }
        
        public List<ActiveStatusEffect> GetActiveEffects(CharacterModel character)
        {
            if (!_activeEffects.TryGetValue(character, out var effects))
            {
                return new List<ActiveStatusEffect>();
            }
            
            return effects.ToList();
        }
        
        public bool HasStatusEffect(CharacterModel character, EffectType effectType)
        {
            if (!_activeEffects.TryGetValue(character, out var effects))
            {
                return false;
            }
            
            return effects.Any(e => e.EffectType == effectType);
        }
        
        public void ClearEffects(CharacterModel character)
        {
            if (_activeEffects.TryGetValue(character, out var effects))
            {
                foreach (var effect in effects.ToList())
                {
                    if (_statusImplementations.TryGetValue(effect.EffectType, out var implementation))
                    {
                        implementation.OnRemoved(character, effect.Intensity);
                    }
                }
                
                effects.Clear();
                _activeEffects.Remove(character);
            }
        }
    }
    
    public class ActiveStatusEffect
    {
        public EffectType EffectType;
        public int Duration;
        public int Intensity;
    }
}