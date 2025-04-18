using SpaceKomodo.TurnBasedSystem.Maps;
using SpaceKomodo.TurnBasedSystem.Views;

namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public interface ITargetIndicatorFactory
    {
        TargetIndicatorView Create(MapCharacterModel model);
    }
}