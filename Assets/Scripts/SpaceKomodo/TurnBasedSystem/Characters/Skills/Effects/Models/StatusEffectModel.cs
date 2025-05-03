using System;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
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
}