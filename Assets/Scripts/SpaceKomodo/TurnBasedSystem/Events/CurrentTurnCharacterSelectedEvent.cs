using SpaceKomodo.TurnBasedSystem.Characters;

namespace SpaceKomodo.TurnBasedSystem.Events
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