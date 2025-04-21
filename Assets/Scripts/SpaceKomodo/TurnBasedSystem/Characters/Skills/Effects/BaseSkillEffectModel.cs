using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    [Serializable]
    public abstract class BaseSkillEffectModel : ICloneable
    {
        public RelativeTarget Target;
        
        public abstract EffectType EffectType { get; }
        
        public abstract object Clone();
    }
}