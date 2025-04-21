using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    [Serializable]
    public class SkillEffectModel : ICloneable
    {
        public EffectType EffectType;
        public RelativeTarget Target;
        public EffectParameters Parameters = new EffectParameters();
    
        public object Clone()
        {
            return new SkillEffectModel
            {
                EffectType = EffectType,
                Target = Target,
                Parameters = Parameters
            };
        }
    }
}