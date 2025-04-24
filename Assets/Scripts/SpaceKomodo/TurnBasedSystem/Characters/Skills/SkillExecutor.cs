using System.Collections.Generic;
using MessagePipe;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using SpaceKomodo.TurnBasedSystem.Events;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public class SkillExecutor
    {
        private readonly EffectRegistry _effectRegistry;
        private readonly IEffectTargetResolver _targetResolver;
        private readonly IPublisher<EffectExecutedEvent> _effectExecutedPublisher;
        
        public SkillExecutor(
            EffectRegistry effectRegistry,
            IEffectTargetResolver targetResolver,
            IPublisher<EffectExecutedEvent> effectExecutedPublisher)
        {
            _effectRegistry = effectRegistry;
            _targetResolver = targetResolver;
            _effectExecutedPublisher = effectExecutedPublisher;
        }
        
        public void ExecuteSkill(CharacterModel source, CharacterModel primaryTarget, SkillModel skill)
        {
            foreach (var effectContainer in skill.Effects)
            {
                var effectModel = effectContainer.GetEffectModel(_effectRegistry);
                var behavior = _effectRegistry.GetBehavior(effectModel.Type);
                var targets = _targetResolver.ResolveTargets(source, primaryTarget, effectModel.Target);
                
                foreach (var target in targets)
                {
                    behavior.Execute(source, target, effectModel);
                    
                    _effectExecutedPublisher.Publish(new EffectExecutedEvent(
                        source, 
                        target, 
                        0));
                }
            }
        }
        
        public Dictionary<CharacterModel, List<Dictionary<string, object>>> PredictSkillEffects(
            CharacterModel source, 
            CharacterModel primaryTarget, 
            SkillModel skill)
        {
            var result = new Dictionary<CharacterModel, List<Dictionary<string, object>>>();
            
            foreach (var effectContainer in skill.Effects)
            {
                var effectModel = effectContainer.GetEffectModel(_effectRegistry);
                var behavior = _effectRegistry.GetBehavior(effectModel.Type);
                var targets = _targetResolver.ResolveTargets(source, primaryTarget, effectModel.Target);
                
                foreach (var target in targets)
                {
                    var prediction = behavior.PredictEffect(source, target, effectModel);
                    
                    if (!result.TryGetValue(target, out var predictions))
                    {
                        predictions = new List<Dictionary<string, object>>();
                        result[target] = predictions;
                    }
                    
                    predictions.Add(prediction);
                }
            }
            
            return result;
        }
    }
}