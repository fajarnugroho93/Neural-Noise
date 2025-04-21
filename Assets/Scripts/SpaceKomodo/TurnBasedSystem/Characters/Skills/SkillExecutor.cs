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
            foreach (var effectContainer in skill.Effects)
            {
                var effectModel = effectContainer.GetEffectModel();
                var effect = _effectFactory.CreateEffect(effectModel.EffectType);
                var targets = effect.GetTargets(source, primaryTarget, effectModel.Target);
                
                foreach (var target in targets)
                {
                    effect.Execute(source, target, effectModel);
                    
                    _effectExecutedPublisher.Publish(new EffectExecutedEvent(
                        source, 
                        target, 
                        null, // Old SkillEffectModel reference, now removed
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
                var effectModel = effectContainer.GetEffectModel();
                var effect = _effectFactory.CreateEffect(effectModel.EffectType);
                var targets = effect.GetTargets(source, primaryTarget, effectModel.Target);
                
                foreach (var target in targets)
                {
                    var prediction = effect.PredictEffect(source, target, effectModel);
                    
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