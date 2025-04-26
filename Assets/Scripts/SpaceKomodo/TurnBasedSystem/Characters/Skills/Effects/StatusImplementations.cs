namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public abstract class BaseStatusImplementation : IStatusEffectImplementation
    {
        public virtual void OnApplied(CharacterModel target, int intensity) { }
        public virtual void OnRemoved(CharacterModel target, int intensity) { }
        public virtual void OnRoundStart(CharacterModel target, int intensity) { }
    }
    
    public class PoisonStatusImplementation : BaseStatusImplementation
    {
        private readonly DamageCalculator _damageCalculator;
        
        public PoisonStatusImplementation(DamageCalculator damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }

        public override void OnRoundStart(CharacterModel target, int intensity)
        {
            _damageCalculator.ApplyDamage(target, intensity, DamageType.Poison);
        }
    }
    
    public class BurnStatusImplementation : BaseStatusImplementation
    {
        private readonly DamageCalculator _damageCalculator;
        
        public BurnStatusImplementation(DamageCalculator damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }
        
        public override void OnRoundStart(CharacterModel target, int intensity)
        {
            _damageCalculator.ApplyDamage(target, intensity, DamageType.Burn);
        }
    }
    
    public class StunStatusImplementation : BaseStatusImplementation
    {
        // Add stun-specific logic
    }
    
    public class BlindStatusImplementation : BaseStatusImplementation
    {
        // Add blind-specific logic
    }
    
    public class SilenceStatusImplementation : BaseStatusImplementation
    {
        // Add silence-specific logic
    }
    
    public class RootStatusImplementation : BaseStatusImplementation
    {
        // Add root-specific logic
    }
    
    public class TauntStatusImplementation : BaseStatusImplementation
    {
        // Add taunt-specific logic
    }
}