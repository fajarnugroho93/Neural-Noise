using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Core;

namespace SpaceKomodo.TurnBasedSystem
{
    public static class BattleModelExtension
    {
        public static void RegisterAllCharacters(this BattleModel battleModel, TurnBasedModel turnBasedModel)
        {
            foreach (var character in turnBasedModel.characterModels)
            {
                battleModel.RegisterCharacter(character);
            }
        }
    }
}