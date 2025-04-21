using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public class PoisonStatusImplementation : IStatusEffectImplementation
    {
        private readonly DamageCalculator _damageCalculator;
        
        public PoisonStatusImplementation(DamageCalculator damageCalculator)
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
            _damageCalculator.ApplyDamage(target, intensity, DamageType.Poison);
        }
    }
}