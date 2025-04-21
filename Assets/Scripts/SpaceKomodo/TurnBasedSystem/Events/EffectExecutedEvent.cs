using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;

namespace SpaceKomodo.TurnBasedSystem.Events
{
    public class EffectExecutedEvent
    {
        public readonly CharacterModel Source;
        public readonly CharacterModel Target;
        public readonly BaseSkillEffectModel Effect;
        public readonly int FinalValue;
        
        public EffectExecutedEvent(
            CharacterModel source, 
            CharacterModel target, 
            BaseSkillEffectModel effect,
            int finalValue)
        {
            Source = source;
            Target = target;
            Effect = effect;
            FinalValue = finalValue;
        }
    }
}