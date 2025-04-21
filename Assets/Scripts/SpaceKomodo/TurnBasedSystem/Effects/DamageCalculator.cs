using R3;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters
{
    public class DamageCalculator
    {
        public int CalculateDamage(CharacterModel source, CharacterModel target, int baseDamage, bool isCritical, DamageType damageType)
        {
            var finalDamage = baseDamage;
            
            if (isCritical)
            {
                finalDamage = Mathf.RoundToInt(finalDamage * 1.5f);
            }
            
            return finalDamage;
        }
        
        public void ApplyDamage(CharacterModel target, int damage, DamageType damageType)
        {
            if (damageType == DamageType.Poison)
            {
                target.CurrentHealth.Value = Mathf.Max(0, target.CurrentHealth.Value - damage);
                return;
            }
            
            var remainingDamage = damage;
            
            if (target.CurrentShield.Value > 0)
            {
                var shieldDamage = Mathf.Min(remainingDamage, target.CurrentShield.Value);
                target.CurrentShield.Value -= shieldDamage;
                remainingDamage -= shieldDamage;
            }
            
            if (remainingDamage > 0)
            {
                target.CurrentHealth.Value = Mathf.Max(0, target.CurrentHealth.Value - remainingDamage);
            }
        }
        
        public bool CalculateCritical(float chance, float multiplier)
        {
            return Random.value < chance;
        }
    }
}