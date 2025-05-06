using System;
using System.Collections.Generic;
using System.Linq;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using SpaceKomodo.TurnBasedSystem.Dice;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    [Serializable]
    public class SkillModel : ICloneable
    {
        public Skill Skill;
        public Sprite Portrait;
        public DiceFaceRequirement DiceFaceRequirement;
        public SkillTarget Target;
        public List<SkillEffectContainer> Effects = new List<SkillEffectContainer>();
    
        public object Clone()
        {
            return new SkillModel
            {
                Skill = Skill,
                Portrait = Portrait,
                Target = Target,
                Effects = Effects?
                              .Select(effectContainer => (SkillEffectContainer)effectContainer.Clone()).ToList() 
                          ?? new List<SkillEffectContainer>()
            };
        }
        
        public void AddEffect(EffectType effectType, EffectRegistry registry)
        {
            var newModel = registry.CreateModel(effectType);
            var container = new SkillEffectContainer();
            container.SetEffectModel(newModel);
            Effects.Add(container);
        }
    }
}