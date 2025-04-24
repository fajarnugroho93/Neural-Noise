using System;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    [Serializable]
    public class InstantEffectModel : IInstantEffect
    {
        public virtual EffectType Type => EffectType.Damage;
        public RelativeTarget Target { get; set; }
        public int Amount { get; set; }
        [Range(0f, 1f)]
        public float CriticalChance { get; set; }
        [Range(1f, 3f)]
        public float CriticalMultiplier { get; set; } = 1.5f;

        public object Clone()
        {
            return new InstantEffectModel
            {
                Target = Target,
                Amount = Amount,
                CriticalChance = CriticalChance,
                CriticalMultiplier = CriticalMultiplier
            };
        }
    }
    
    [Serializable]
    public class DamageEffectModel : InstantEffectModel
    {
        public override EffectType Type => EffectType.Damage;
    }
    
    [Serializable]
    public class HealEffectModel : InstantEffectModel
    {
        public override EffectType Type => EffectType.Heal;
    }

    [Serializable]
    public class StatusEffectModel : IStatusEffect
    {
        public virtual EffectType Type => EffectType.Damage;
        public RelativeTarget Target { get; set; }
        public int Amount { get; set; }
        public int Duration { get; set; }
        [Range(0f, 1f)]
        public float CriticalChance { get; set; }
        [Range(1f, 3f)]
        public float CriticalMultiplier { get; set; } = 1.5f;

        public object Clone()
        {
            return new StatusEffectModel
            {
                Target = Target,
                Amount = Amount,
                Duration = Duration,
                CriticalChance = CriticalChance,
                CriticalMultiplier = CriticalMultiplier
            };
        }
    }
    
    [Serializable]
    public class ShieldEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Shield;
    }
    
    [Serializable]
    public class PoisonEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Poison;
    }
    
    [Serializable]
    public class BurnEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Burn;
    }
    
    [Serializable]
    public class StunEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Stun;
    }
    
    [Serializable]
    public class EnergyEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Energy;
    }
    
    [Serializable]
    public class RageEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Rage;
    }
}