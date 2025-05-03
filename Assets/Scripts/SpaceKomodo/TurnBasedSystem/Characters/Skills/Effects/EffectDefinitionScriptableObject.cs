using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    [CreateAssetMenu(fileName = "New Effect Definition", menuName = "TurnBasedSystem/Effect Definition")]
    public class EffectDefinitionScriptableObject : ScriptableObject
    {
        public EffectType EffectType;
        public EffectCategory Category;

        [SerializeReference]
        public IEffectModel ModelImplementation;
        
        [SerializeReference]
        public IEffectBehavior BehaviorImplementation;
        
        [Header("Default Properties")]
        public int DefaultAmount = 10;
        public int DefaultDuration = 3;
        public float DefaultCriticalChance = 0.1f;
        public float DefaultCriticalMultiplier = 1.5f;
    }
}