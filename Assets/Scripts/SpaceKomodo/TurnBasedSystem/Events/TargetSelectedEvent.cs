using SpaceKomodo.TurnBasedSystem.Characters;

namespace SpaceKomodo.TurnBasedSystem.Events
{
    public class TargetSelectedEvent
    {
        public readonly CharacterModel Target;
        
        public TargetSelectedEvent(CharacterModel target)
        {
            Target = target;
        }
    }
}