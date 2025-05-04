using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class BasicHealModel : BasicEffectModel
    {
        public override EffectType Type => EffectType.Heal;
    }
}