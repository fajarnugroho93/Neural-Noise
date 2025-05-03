using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors
{
    public class PoisonStatusImplementation : BaseStatusImplementation
    {
        private readonly DamageCalculator _damageCalculator;

        public PoisonStatusImplementation(EffectType effectType, DamageCalculator damageCalculator) : base(effectType)
        {
            _damageCalculator = damageCalculator;
        }

        public override void OnRoundStart(CharacterModel target, int intensity)
        {
            _damageCalculator.ApplyDamage(target, intensity, DamageType.Poison);
        }
    }
}