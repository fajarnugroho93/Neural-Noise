using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public class BurnStatusImplementation : IStatusEffectImplementation
    {
        private readonly DamageCalculator _damageCalculator;
        
        public BurnStatusImplementation(DamageCalculator damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }
        
        public void OnApplied(CharacterModel target, int intensity)
        {
        }
        
        public void OnRemoved(CharacterModel target, int intensity)
        {
        }
        
        public void OnRoundStart(CharacterModel target, int intensity)
        {
            _damageCalculator.ApplyDamage(target, intensity, DamageType.Burn);
        }
    }
}