using UnityEngine;

namespace TurnBasedSystem.Characters.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "TurnBasedSystem/Skill", order = -10000)]
    public class SkillScriptableObject : ScriptableObject
    {
        public SkillModel SkillModel;
    }
}