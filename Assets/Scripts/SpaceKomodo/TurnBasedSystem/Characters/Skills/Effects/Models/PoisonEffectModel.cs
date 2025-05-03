using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class PoisonEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Poison;
    }
}