using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class ShieldEffect : BaseSkillEffect
    {
        public override EffectType Type => EffectType.Shield;
        
        public override void Execute(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel)
        {
            var shieldModel = (ShieldEffectModel)effectModel;
            primaryTarget.CurrentShield.Value += shieldModel.Amount;
        }
        
        public override Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            var shieldModel = (ShieldEffectModel)effectModel;
            
            result["ShieldAmount"] = shieldModel.Amount;
            result["Duration"] = shieldModel.Duration;
            
            return result;
        }
    }
}