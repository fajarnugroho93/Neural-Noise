using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class StunEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Stun;
    }
}