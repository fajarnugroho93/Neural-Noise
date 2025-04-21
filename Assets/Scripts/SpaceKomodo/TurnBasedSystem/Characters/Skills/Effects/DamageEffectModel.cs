using System;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    [Serializable]
    public class DamageEffectModel : BaseSkillEffectModel
    {
        public int Amount;
        [Range(0f, 1f)]
        public float CriticalChance;
        [Range(1f, 3f)]
        public float CriticalMultiplier = 1.5f;
        public DamageType DamageType;
        
        public override EffectType EffectType => EffectType.Damage;
        
        public override object Clone()
        {
            return new DamageEffectModel
            {
                Target = Target,
                Amount = Amount,
                CriticalChance = CriticalChance,
                CriticalMultiplier = CriticalMultiplier,
                DamageType = DamageType
            };
        }
    }
}