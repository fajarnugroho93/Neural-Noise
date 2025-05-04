using System.Collections.Generic;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors
{
    public class BasicShieldBehavior : IEffectBehavior
    {
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is StatusEffectModel shieldModel))
                return;

            target.CurrentShield.Value += shieldModel.Amount;
        }

        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is StatusEffectModel shieldModel))
                return result;

            result["ShieldAmount"] = shieldModel.Amount;
            result["Duration"] = shieldModel.Duration;

            return result;
        }
    }
}