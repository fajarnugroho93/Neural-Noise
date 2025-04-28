using System.Collections.Generic;
using System.Linq;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;

namespace SpaceKomodo.TurnBasedSystem.Effects
{
    public class StatusEffectManager
    {
        private readonly Dictionary<CharacterModel, List<ActiveStatusEffect>> _activeEffects = new();
        private readonly Dictionary<int, IStatusEffectImplementation> _statusImplementations = new();
        
        public void RegisterStatusImplementation(int statusType, IStatusEffectImplementation implementation)
        {
            _statusImplementations[statusType] = implementation;
        }
        
        public void ApplyStatus(CharacterModel target, int statusType, int duration, int intensity)
        {
            if (!_activeEffects.TryGetValue(target, out var effectsList))
            {
                effectsList = new List<ActiveStatusEffect>();
                _activeEffects[target] = effectsList;
            }
            
            var existingEffect = effectsList.FirstOrDefault(e => e.StatusType == statusType);
            
            if (existingEffect != null)
            {
                existingEffect.Duration = duration;
                existingEffect.Intensity += intensity;
            }
            else
            {
                effectsList.Add(new ActiveStatusEffect
                {
                    StatusType = statusType,
                    Duration = duration,
                    Intensity = intensity
                });
                
                if (_statusImplementations.TryGetValue(statusType, out var implementation))
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
                    if (_statusImplementations.TryGetValue(effect.StatusType, out var implementation))
                    {
                        implementation.OnRoundStart(character, effect.Intensity);
                    }
                    
                    effect.Duration--;
                    
                    if (effect.Duration <= 0)
                    {
                        if (_statusImplementations.TryGetValue(effect.StatusType, out implementation))
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
        
        public bool HasStatusEffect(CharacterModel character, int statusType)
        {
            if (!_activeEffects.TryGetValue(character, out var effects))
            {
                return false;
            }
            
            return effects.Any(e => e.StatusType == statusType);
        }
        
        public void ClearEffects(CharacterModel character)
        {
            if (_activeEffects.TryGetValue(character, out var effects))
            {
                foreach (var effect in effects.ToList())
                {
                    if (_statusImplementations.TryGetValue(effect.StatusType, out var implementation))
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
        public int StatusType;
        public int Duration;
        public int Intensity;
    }
}