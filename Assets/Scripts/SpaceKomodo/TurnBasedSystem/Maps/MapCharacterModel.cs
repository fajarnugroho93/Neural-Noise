using SpaceKomodo.TurnBasedSystem.Characters;

namespace SpaceKomodo.TurnBasedSystem.Maps
{
    public class MapCharacterModel
    {
        public CharacterModel CharacterModel;
        public MapGridModel MapPositions;

        public MapCharacterModel(
            CharacterModel characterModel, 
            MapGridModel mapPositions)
        {
            CharacterModel = characterModel;
            MapPositions = mapPositions;
        }
    }
}