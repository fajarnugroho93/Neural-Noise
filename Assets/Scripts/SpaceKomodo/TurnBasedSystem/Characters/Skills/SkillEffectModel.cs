using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    [Serializable]
    public class SkillEffectModel : ICloneable
    {
        public SkillEffect Effect;
        public int Value;
    
        public object Clone()
        {
            return new SkillEffectModel
            {
                Effect = Effect,
                Value = Value
            };
        }
    }
}