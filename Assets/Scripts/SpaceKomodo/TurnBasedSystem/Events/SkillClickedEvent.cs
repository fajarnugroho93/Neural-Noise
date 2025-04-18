using SpaceKomodo.TurnBasedSystem.Characters.Skills;

namespace SpaceKomodo.TurnBasedSystem.Events
{
    public class SkillClickedEvent
    {
        public readonly SkillModel Skill;
        
        public SkillClickedEvent(SkillModel skill)
        {
            Skill = skill;
        }
    }
}