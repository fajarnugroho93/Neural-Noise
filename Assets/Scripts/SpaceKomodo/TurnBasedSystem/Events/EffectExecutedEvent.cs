using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;

namespace SpaceKomodo.TurnBasedSystem.Events
{
    public class EffectExecutedEvent
    {
        public readonly CharacterModel Source;
        public readonly CharacterModel Target;
        public readonly int FinalValue;
        
        public EffectExecutedEvent(
            CharacterModel source, 
            CharacterModel target, 
            int finalValue)
        {
            Source = source;
            Target = target;
            FinalValue = finalValue;
        }
    }
}