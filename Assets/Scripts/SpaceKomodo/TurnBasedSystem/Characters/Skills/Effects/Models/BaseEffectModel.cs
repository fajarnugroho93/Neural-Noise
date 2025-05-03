using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public abstract class BaseEffectModel : IEffectModel
    {
        public abstract EffectType Type { get; }
        public RelativeTarget Target { get; set; }
        
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}