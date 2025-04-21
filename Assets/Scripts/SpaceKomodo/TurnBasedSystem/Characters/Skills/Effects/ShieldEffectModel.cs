using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    [Serializable]
    public class ShieldEffectModel : BaseSkillEffectModel
    {
        public int Amount;
        public int Duration;
        
        public override EffectType EffectType => EffectType.Shield;
        
        public override object Clone()
        {
            return new ShieldEffectModel
            {
                Target = Target,
                Amount = Amount,
                Duration = Duration
            };
        }
    }
}