using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    [Serializable]
    public class SkillModel : ICloneable
    {
        public Skill Skill;
        public Sprite Portrait;
        public SkillTarget Target;
        public List<SkillEffectModel> Effects;
    
        public object Clone()
        {
            return new SkillModel
            {
                Skill = Skill,
                Portrait = Portrait,
                Target = Target,
                Effects = Effects?
                    .Select(skillEffectModel => (SkillEffectModel)skillEffectModel.Clone()).ToList() ?? new List<SkillEffectModel>()
            };
        }
    }
}