using System.Collections.Generic;
using R3;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public class ShieldEffect : BaseSkillEffect
    {
        public override EffectType Type => EffectType.Shield;
        
        public override void Execute(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters)
        {
            var shieldAmount = parameters.GetInt("Amount", 0);
            
            primaryTarget.CurrentShield.Value += shieldAmount;
        }
        
        public override Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters)
        {
            var result = new Dictionary<string, object>();
            
            var shieldAmount = parameters.GetInt("Amount", 0);
            
            result["ShieldAmount"] = shieldAmount;
            
            return result;
        }
    }
}