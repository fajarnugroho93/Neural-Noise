using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public static class SkillEffectModelFactory
    {
        public static BaseSkillEffectModel CreateDefaultModel(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.Damage:
                    return new DamageEffectModel 
                    { 
                        Target = RelativeTarget.Primary,
                        Amount = 10,
                        CriticalChance = 0.1f,
                        CriticalMultiplier = 1.5f,
                        DamageType = DamageType.Normal
                    };
                
                case EffectType.Heal:
                    return new HealEffectModel
                    {
                        Target = RelativeTarget.Self,
                        Amount = 15,
                        CriticalChance = 0.1f,
                        CriticalMultiplier = 1.5f
                    };
                
                case EffectType.Shield:
                    return new ShieldEffectModel
                    {
                        Target = RelativeTarget.Self,
                        Amount = 10,
                        Duration = 3
                    };
                
                case EffectType.Status:
                    return new StatusEffectModel
                    {
                        Target = RelativeTarget.Primary,
                        StatusType = StatusType.Poison,
                        Duration = 3,
                        Intensity = 5
                    };
                
                case EffectType.Resource:
                    return new ResourceEffectModel
                    {
                        Target = RelativeTarget.Self,
                        ResourceType = ResourceType.Energy,
                        Amount = 10
                    };
                
                default:
                    Debug.LogError($"Unknown effect type: {effectType}");
                    return null;
            }
        }
    }
}