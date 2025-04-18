namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public abstract class TurnCommand
    {
        public abstract bool CanExecute();
        public abstract void Execute();
        public abstract void Undo();
        public abstract string GetDescription();
    }
}