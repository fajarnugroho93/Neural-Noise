using System;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models
{
    [Serializable]
    public class BasicDamageModel : BasicEffectModel
    {
        public override EffectType Type => EffectType.Damage;
    }
}