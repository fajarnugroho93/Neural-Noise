using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using SpaceKomodo.TurnBasedSystem.Effects;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class SkillModelDrawer : UnityEditor.Editor
    {
        private EffectRegistry _effectRegistry;
        
        private void OnEnable()
        {
            CreateEffectRegistry();
        }
        
        private void CreateEffectRegistry()
        {
            var damageCalculator = new DamageCalculator();
            var resourceManager = new ResourceManager();
            var statusEffectManager = new StatusEffectManager();
            
            var effectRegistries = Resources.Load<EffectRegistriesScriptableObject>("Data/Effects");
            _effectRegistry = new EffectRegistry(damageCalculator, statusEffectManager, resourceManager, effectRegistries);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawDefaultInspector();
            
            serializedObject.ApplyModifiedProperties();
            
            var monoBehaviour = target as MonoBehaviour;
            if (monoBehaviour == null) return;
            
            EditorGUILayout.Space();
            
            var propertyFields = monoBehaviour.GetType().GetFields();
            
            foreach (var field in propertyFields)
            {
                if (field.FieldType == typeof(SkillModel))
                {
                    var skillModel = field.GetValue(monoBehaviour) as SkillModel;
                    if (skillModel == null) continue;
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(field.Name, EditorStyles.boldLabel);
                    
                    if (GUILayout.Button("Add Effect"))
                    {
                        GenericMenu menu = new GenericMenu();
                        
                        AddEffectMenuItem(menu, "Basic Effects");
                        AddEffectMenuItem(menu, "Status Effects");
                        AddEffectMenuItem(menu, "Resource Effects");
                        
                        menu.ShowAsContext();
                    }
                }
            }
        }

        private void AddEffectMenuItem(GenericMenu menu, string categoryName)
        {
            var effectRegistries = Resources.Load<EffectRegistriesScriptableObject>("Data/Effects");
            if (effectRegistries == null)
                return;

            EffectRegistryScriptableObject[] effectsArray = null;
            
            switch (categoryName)
            {
                case "Basic Effects":
                    effectsArray = effectRegistries.BasicEffects;
                    break;
                case "Status Effects":
                    effectsArray = effectRegistries.StatusEffects;
                    break;
                case "Resource Effects":
                    effectsArray = effectRegistries.ResourceEffects;
                    break;
            }

            if (effectsArray == null || effectsArray.Length == 0)
                return;

            foreach (var effect in effectsArray)
            {
                string effectName = effect.EffectType.ToString();
                menu.AddItem(new GUIContent($"{categoryName}/{effectName}"), false, () => 
                {
                    var monoBehaviour = target as MonoBehaviour;
                    if (monoBehaviour == null) return;
                    
                    foreach (var field in monoBehaviour.GetType().GetFields())
                    {
                        if (field.FieldType == typeof(SkillModel))
                        {
                            var skillModel = field.GetValue(monoBehaviour) as SkillModel;
                            if (skillModel == null) continue;
                            
                            skillModel.AddEffect(effect.EffectType, _effectRegistry);
                            EditorUtility.SetDirty(target);
                            break;
                        }
                    }
                });
            }
        }
        
        private void AddEffectMenuItem(GenericMenu menu, string path, EffectType effectType, SkillModel skillModel)
        {
            menu.AddItem(new GUIContent(path), false, () => 
            {
                skillModel.AddEffect(effectType, _effectRegistry);
                EditorUtility.SetDirty(target);
            });
        }
    }
}