using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;

namespace SpaceKomodo.TurnBasedSystem.Events
{
    public class EffectExecutedEvent
    {
        public readonly CharacterModel Source;
        public readonly CharacterModel Target;
        public readonly SkillEffectModel Effect;
        public readonly int FinalValue;
        
        public EffectExecutedEvent(
            CharacterModel source, 
            CharacterModel target, 
            SkillEffectModel effect,
            int finalValue)
        {
            Source = source;
            Target = target;
            Effect = effect;
            FinalValue = finalValue;
        }
    }
}