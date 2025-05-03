using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class DamageEffectModel : InstantEffectModel
    {
        public override EffectType Type => EffectType.Damage;
    }
}