using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class BurnEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Burn;
    }
}