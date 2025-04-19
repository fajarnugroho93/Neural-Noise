namespace SpaceKomodo.TurnBasedSystem.Core
{
    public enum TurnPhase
    {
        Idle,
        SelectSkill,
        SelectTarget,
        Confirmation,
        Execute,
        NextTurn
    }
}