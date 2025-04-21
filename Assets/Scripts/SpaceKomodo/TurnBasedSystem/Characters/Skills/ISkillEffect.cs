using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public interface ISkillEffect
    {
        EffectType Type { get; }
        
        void Execute(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters);
        
        List<CharacterModel> GetTargets(CharacterModel source, CharacterModel primaryTarget, RelativeTarget relativeTarget);
        
        Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters);
    }
}