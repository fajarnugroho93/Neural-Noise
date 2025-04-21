using System;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    [Serializable]
    public class HealEffectModel : BaseSkillEffectModel
    {
        public int Amount;
        [Range(0f, 1f)]
        public float CriticalChance;
        [Range(1f, 3f)]
        public float CriticalMultiplier = 1.5f;
        
        public override EffectType EffectType => EffectType.Heal;
        
        public override object Clone()
        {
            return new HealEffectModel
            {
                Target = Target,
                Amount = Amount,
                CriticalChance = CriticalChance,
                CriticalMultiplier = CriticalMultiplier
            };
        }
    }
}