using SpaceKomodo.TurnBasedSystem.Characters.Skills;

namespace SpaceKomodo.TurnBasedSystem.Events
{
    public class SkillSelectedEvent
    {
        public readonly SkillModel Skill;
        
        public SkillSelectedEvent(SkillModel skill)
        {
            Skill = skill;
        }
    }
}