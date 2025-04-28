using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
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
}