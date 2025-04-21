using System;
using System.Collections.Generic;
using System.Linq;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    [Serializable]
    public class SkillModel : ICloneable
    {
        public Skill Skill;
        public Sprite Portrait;
        public SkillTarget Target;
        public List<SkillEffectModelContainer> Effects = new List<SkillEffectModelContainer>();
    
        public object Clone()
        {
            return new SkillModel
            {
                Skill = Skill,
                Portrait = Portrait,
                Target = Target,
                Effects = Effects?
                              .Select(effectContainer => (SkillEffectModelContainer)effectContainer.Clone()).ToList() 
                          ?? new List<SkillEffectModelContainer>()
            };
        }
        
        public void AddEffect(BaseSkillEffectModel effectModel)
        {
            var container = new SkillEffectModelContainer();
            container.SetEffectModel(effectModel);
            Effects.Add(container);
        }
    }
}