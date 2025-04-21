using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class StatusEffect : BaseSkillEffect
    {
        private readonly StatusEffectManager _statusEffectManager;
        
        public StatusEffect(StatusEffectManager statusEffectManager)
        {
            _statusEffectManager = statusEffectManager;
        }
        
        public override EffectType Type => EffectType.Status;
        
        public override void Execute(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel)
        {
            var statusModel = (StatusEffectModel)effectModel;
            _statusEffectManager.ApplyStatus(primaryTarget, (int)statusModel.StatusType, statusModel.Duration, statusModel.Intensity);
        }
        
        public override Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            var statusModel = (StatusEffectModel)effectModel;
            
            result["StatusType"] = statusModel.StatusType;
            result["Duration"] = statusModel.Duration;
            result["Intensity"] = statusModel.Intensity;
            
            return result;
        }
    }
}