using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class ShieldEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Shield;
    }
}