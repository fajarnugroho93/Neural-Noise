using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors
{
    public class StatusBurnBehavior : BaseStatusBehavior
    {
        private readonly DamageCalculator _damageCalculator;

        public StatusBurnBehavior(EffectType effectType, DamageCalculator damageCalculator) : base(effectType)
        {
            _damageCalculator = damageCalculator;
        }

        public override void OnRoundStart(CharacterModel target, int intensity)
        {
            _damageCalculator.ApplyDamage(target, intensity, DamageType.Burn);
        }
    }
}