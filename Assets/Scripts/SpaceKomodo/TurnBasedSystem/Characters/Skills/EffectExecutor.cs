using System.Collections.Generic;
using MessagePipe;
using SpaceKomodo.TurnBasedSystem.Events;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public class EffectExecutor : IEffectExecutor
    {
        private readonly IPublisher<EffectExecutedEvent> _effectExecutedPublisher;
        
        public EffectExecutor(IPublisher<EffectExecutedEvent> effectExecutedPublisher)
        {
            _effectExecutedPublisher = effectExecutedPublisher;
        }
        
        public void ExecuteEffect(CharacterModel source, CharacterModel target, SkillEffectModel effect)
        {
            var finalValue = CalculateEffectValue(source, target, effect);
            
            switch (effect.Effect)
            {
                case SkillEffect.TargetDamage:
                    target.CurrentHealth.Value = Mathf.Max(0, target.CurrentHealth.Value - finalValue);
                    break;
                case SkillEffect.TargetHeal:
                    target.CurrentHealth.Value = Mathf.Min(target.CurrentMaxHealth.Value, target.CurrentHealth.Value + finalValue);
                    break;
                case SkillEffect.TargetShield:
                    // A shield could be implemented as a temporary health bonus
                    // or as a separate shield value on the CharacterModel
                    break;
                case SkillEffect.SelfDamage:
                    source.CurrentHealth.Value = Mathf.Max(0, source.CurrentHealth.Value - finalValue);
                    break;
                case SkillEffect.SelfHeal:
                    source.CurrentHealth.Value = Mathf.Min(source.CurrentMaxHealth.Value, source.CurrentHealth.Value + finalValue);
                    break;
                case SkillEffect.SelfShield:
                    // Same as target shield
                    break;
                case SkillEffect.TargetPoison:
                    // Apply poison status effect to target
                    // This would need a status effect system
                    break;
                case SkillEffect.TargetBurn:
                    // Apply burn status effect to target
                    break;
            }
            
            _effectExecutedPublisher.Publish(new EffectExecutedEvent(source, target, effect, finalValue));
        }
        
        public Dictionary<SkillEffect, int> PredictEffects(CharacterModel source, CharacterModel target, List<SkillEffectModel> effects)
        {
            var predictions = new Dictionary<SkillEffect, int>();
            
            foreach (var effect in effects)
            {
                var finalValue = CalculateEffectValue(source, target, effect);
                predictions[effect.Effect] = finalValue;
            }
            
            return predictions;
        }
        
        private int CalculateEffectValue(CharacterModel source, CharacterModel target, SkillEffectModel effect)
        {
            return effect.Value;
        }
    }
}