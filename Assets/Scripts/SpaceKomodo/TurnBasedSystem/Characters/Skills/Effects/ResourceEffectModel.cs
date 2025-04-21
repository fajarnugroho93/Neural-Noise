using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    [Serializable]
    public class ResourceEffectModel : BaseSkillEffectModel
    {
        public ResourceType ResourceType;
        public int Amount;
        
        public override EffectType EffectType => EffectType.Resource;
        
        public override object Clone()
        {
            return new ResourceEffectModel
            {
                Target = Target,
                ResourceType = ResourceType,
                Amount = Amount
            };
        }
    }
}