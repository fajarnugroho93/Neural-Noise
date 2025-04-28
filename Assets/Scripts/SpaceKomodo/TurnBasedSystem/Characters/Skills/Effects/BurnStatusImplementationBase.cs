using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
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
}