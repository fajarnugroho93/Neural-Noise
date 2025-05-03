using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class HealEffectModel : InstantEffectModel
    {
        public override EffectType Type => EffectType.Heal;
    }
}