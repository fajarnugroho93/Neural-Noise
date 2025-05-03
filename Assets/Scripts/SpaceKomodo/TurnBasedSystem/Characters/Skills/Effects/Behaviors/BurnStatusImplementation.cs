using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors
{
    public class BurnStatusImplementation : BaseStatusImplementation
    {
        private readonly DamageCalculator _damageCalculator;

        public BurnStatusImplementation(EffectType effectType, DamageCalculator damageCalculator) : base(effectType)
        {
            _damageCalculator = damageCalculator;
        }

        public override void OnRoundStart(CharacterModel target, int intensity)
        {
            _damageCalculator.ApplyDamage(target, intensity, DamageType.Burn);
        }
    }
}