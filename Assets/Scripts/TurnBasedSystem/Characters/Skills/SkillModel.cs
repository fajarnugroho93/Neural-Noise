using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedSystem.Characters.Skills
{
    [Serializable]
    public class SkillModel
    {
        public Skill Skill;
        public Sprite Portrait;
        public SkillTarget Target;
        public List<SkillEffectModel> Effects;
    }
}