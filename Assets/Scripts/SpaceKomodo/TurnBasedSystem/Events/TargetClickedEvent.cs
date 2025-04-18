using SpaceKomodo.TurnBasedSystem.Characters;

namespace SpaceKomodo.TurnBasedSystem.Events
{
    public class TargetClickedEvent
    {
        public readonly CharacterModel Target;
        
        public TargetClickedEvent(CharacterModel target)
        {
            Target = target;
        }
    }
}