namespace SpaceKomodo.TurnBasedSystem.Core
{
    public enum TurnPhase
    {
        Idle,
        SelectDice,
        SelectSkill,
        SelectTarget,
        Confirmation,
        Execute,
        NextTurn
    }
}