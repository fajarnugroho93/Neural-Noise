using System.Collections.Generic;
using SpaceKomodo.TurnBasedSystem.Characters;

namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public interface ITargetIndicatorManager
    {
        void UpdateTargetIndicators(IReadOnlyList<CharacterModel> validTargets);
        void SetSelectedTarget(CharacterModel target);
        void ClearTargetIndicators();
    }
}