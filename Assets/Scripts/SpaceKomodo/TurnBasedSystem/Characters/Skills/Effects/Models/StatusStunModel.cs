using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class StatusStunModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Stun;
    }
}