using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors
{
    public class NoneBehavior : IEffectBehavior
    {
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            
        }

        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();

            return result;
        }
    }
}