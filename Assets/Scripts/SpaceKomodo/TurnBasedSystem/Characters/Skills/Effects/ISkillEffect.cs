using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public interface ISkillEffect
    {
        EffectType Type { get; }
        
        void Execute(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel);
        
        List<CharacterModel> GetTargets(CharacterModel source, CharacterModel primaryTarget, RelativeTarget relativeTarget);
        
        Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel);
    }
}