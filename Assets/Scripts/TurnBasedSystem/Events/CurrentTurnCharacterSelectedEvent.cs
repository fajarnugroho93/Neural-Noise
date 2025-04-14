using TurnBasedSystem.Characters;

namespace TurnBasedSystem.Events
{
    public class CurrentTurnCharacterSelectedEvent
    {
        public readonly CharacterModel CharacterModel;

        public CurrentTurnCharacterSelectedEvent(CharacterModel characterModel)
        {
            CharacterModel = characterModel;
        }
    }
}