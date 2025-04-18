using SpaceKomodo.TurnBasedSystem.Commands;

namespace SpaceKomodo.TurnBasedSystem.Events
{
    public class CommandExecutedEvent
    {
        public readonly TurnCommand Command;
        
        public CommandExecutedEvent(TurnCommand command)
        {
            Command = command;
        }
    }
}