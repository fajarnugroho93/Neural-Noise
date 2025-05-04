using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class StatusPoisonModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Poison;
    }
}