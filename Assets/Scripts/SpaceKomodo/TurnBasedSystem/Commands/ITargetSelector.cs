using System.Collections.Generic;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;

namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public interface ITargetSelector
    {
        void SetValidTargets(CharacterModel source, SkillModel skill);
        bool IsValidTarget(CharacterModel target);
        List<CharacterModel> GetValidTargets();
        void ClearValidTargets();
        void SetSelectedTarget(CharacterModel evtTarget);
    }
}