using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public class StatusEffect : BaseSkillEffect
    {
        private readonly StatusEffectManager _statusEffectManager;
        
        public StatusEffect(StatusEffectManager statusEffectManager)
        {
            _statusEffectManager = statusEffectManager;
        }
        
        public override EffectType Type => EffectType.Status;
        
        public override void Execute(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters)
        {
            var statusType = parameters.GetInt("StatusType", 0);
            var duration = parameters.GetInt("Duration", 1);
            var intensity = parameters.GetInt("Intensity", 1);
            
            _statusEffectManager.ApplyStatus(primaryTarget, statusType, duration, intensity);
        }
        
        public override Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters)
        {
            var result = new Dictionary<string, object>();
            
            var statusType = parameters.GetInt("StatusType", 0);
            var duration = parameters.GetInt("Duration", 1);
            var intensity = parameters.GetInt("Intensity", 1);
            
            result["StatusType"] = statusType;
            result["Duration"] = duration;
            result["Intensity"] = intensity;
            
            return result;
        }
    }
}