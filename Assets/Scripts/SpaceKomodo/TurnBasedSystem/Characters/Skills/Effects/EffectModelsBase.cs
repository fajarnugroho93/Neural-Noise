using System;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
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
    
    [Serializable]
    public abstract class InstantEffectModel : BaseEffectModel, IInstantEffect
    {
        public int Amount { get; set; }
        [Range(0f, 1f)]
        public float CriticalChance { get; set; }
        [Range(1f, 3f)]
        public float CriticalMultiplier { get; set; } = 1.5f;
        
        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
    
    [Serializable]
    public abstract class StatusEffectModel : BaseEffectModel, IStatusEffect
    {
        public int Amount { get; set; }
        public int Duration { get; set; }
        [Range(0f, 1f)]
        public float CriticalChance { get; set; }
        [Range(1f, 3f)]
        public float CriticalMultiplier { get; set; } = 1.5f;
        
        public override object Clone()
        {
            return MemberwiseClone();
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
    public class BlindEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Blind;
    }
    
    [Serializable]
    public class SilenceEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Silence;
    }
    
    [Serializable]
    public class RootEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Root;
    }
    
    [Serializable]
    public class TauntEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Taunt;
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
    
    [Serializable]
    public class ManaEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Mana;
    }
    
    [Serializable]
    public class FocusEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Focus;
    }
    
    [Serializable]
    public class ChargeEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Charge;
    }
}