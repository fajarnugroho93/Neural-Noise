using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class RageEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Rage;
    }
}