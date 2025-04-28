using System.Collections.Generic;
using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class StatusBehavior : IEffectBehavior
    {
        private readonly StatusEffectManager _statusEffectManager;
        
        public StatusBehavior(StatusEffectManager statusEffectManager)
        {
            _statusEffectManager = statusEffectManager;
        }
        
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is IStatusEffect statusModel))
                return;
                
            _statusEffectManager.ApplyStatus(
                target, 
                (int)effectModel.Type, 
                statusModel.Duration, 
                statusModel.Amount);
        }
        
        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is IStatusEffect statusModel))
                return result;
                
            result["StatusType"] = effectModel.Type;
            result["Duration"] = statusModel.Duration;
            result["Intensity"] = statusModel.Amount;
            
            return result;
        }
    }
}