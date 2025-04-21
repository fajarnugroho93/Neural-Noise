using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    [Serializable]
    public class StatusEffectModel : BaseSkillEffectModel
    {
        public StatusType StatusType;
        public int Duration;
        public int Intensity;
        
        public override EffectType EffectType => EffectType.Status;
        
        public override object Clone()
        {
            return new StatusEffectModel
            {
                Target = Target,
                StatusType = StatusType,
                Duration = Duration,
                Intensity = Intensity
            };
        }
    }
}