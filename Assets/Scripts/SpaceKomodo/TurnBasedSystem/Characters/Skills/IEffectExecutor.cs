using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public interface IEffectExecutor
    {
        void ExecuteEffect(CharacterModel source, CharacterModel target, SkillEffectModel effect);
        Dictionary<SkillEffect, int> PredictEffects(CharacterModel source, CharacterModel target, List<SkillEffectModel> effects);
    }
}