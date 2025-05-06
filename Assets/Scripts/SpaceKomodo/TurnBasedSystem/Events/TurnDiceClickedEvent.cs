using SpaceKomodo.TurnBasedSystem.Dice;

namespace SpaceKomodo.TurnBasedSystem.Events
{
    public class TurnDiceClickedEvent
    {
        public DiceModel DiceModel;

        public TurnDiceClickedEvent(DiceModel diceModel)
        {
            DiceModel = diceModel;
        }
    }
}