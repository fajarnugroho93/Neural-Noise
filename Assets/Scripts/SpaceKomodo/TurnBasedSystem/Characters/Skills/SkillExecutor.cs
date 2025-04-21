using System.Collections.Generic;
using MessagePipe;
using SpaceKomodo.TurnBasedSystem.Core;
using SpaceKomodo.TurnBasedSystem.Events;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public class SkillExecutor
    {
        private readonly EffectFactory _effectFactory;
        private readonly BattleModel _battleModel;
        private readonly IPublisher<EffectExecutedEvent> _effectExecutedPublisher;
        
        public SkillExecutor(
            EffectFactory effectFactory,
            BattleModel battleModel,
            IPublisher<EffectExecutedEvent> effectExecutedPublisher)
        {
            _effectFactory = effectFactory;
            _battleModel = battleModel;
            _effectExecutedPublisher = effectExecutedPublisher;
        }
        
        public void ExecuteSkill(CharacterModel source, CharacterModel primaryTarget, SkillModel skill)
        {
            foreach (var effectModel in skill.Effects)
            {
                var effect = _effectFactory.CreateEffect(effectModel.EffectType);
                var targets = effect.GetTargets(source, primaryTarget, effectModel.Target);
                
                foreach (var target in targets)
                {
                    effect.Execute(source, target, effectModel.Parameters);
                    
                    _effectExecutedPublisher.Publish(new EffectExecutedEvent(
                        source, 
                        target, 
                        effectModel,
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
            
            foreach (var effectModel in skill.Effects)
            {
                var effect = _effectFactory.CreateEffect(effectModel.EffectType);
                var targets = effect.GetTargets(source, primaryTarget, effectModel.Target);
                
                foreach (var target in targets)
                {
                    var prediction = effect.PredictEffect(source, target, effectModel.Parameters);
                    
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