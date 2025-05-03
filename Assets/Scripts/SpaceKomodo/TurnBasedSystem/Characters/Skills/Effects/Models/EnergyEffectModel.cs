using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class EnergyEffectModel : StatusEffectModel
    {
        public override EffectType Type => EffectType.Energy;
    }
}