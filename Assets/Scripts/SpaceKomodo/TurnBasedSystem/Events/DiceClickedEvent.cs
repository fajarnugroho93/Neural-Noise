using SpaceKomodo.TurnBasedSystem.Dice;

namespace SpaceKomodo.TurnBasedSystem.Events
{
    public class DiceClickedEvent
    {
        public DiceModel DiceModel;

        public DiceClickedEvent(DiceModel diceModel)
        {
            DiceModel = diceModel;
        }
    }
}