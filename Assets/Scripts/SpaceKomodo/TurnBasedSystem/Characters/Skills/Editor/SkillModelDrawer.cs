using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class SkillModelDrawer : UnityEditor.Editor
    {
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
                    
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Damage Effect"))
                    {
                        AddEffect(skillModel, EffectType.Damage);
                    }
                    
                    if (GUILayout.Button("Add Heal Effect"))
                    {
                        AddEffect(skillModel, EffectType.Heal);
                    }
                    
                    if (GUILayout.Button("Add Shield Effect"))
                    {
                        AddEffect(skillModel, EffectType.Shield);
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Status Effect"))
                    {
                        AddEffect(skillModel, EffectType.Status);
                    }
                    
                    if (GUILayout.Button("Add Resource Effect"))
                    {
                        AddEffect(skillModel, EffectType.Resource);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        
        private void AddEffect(SkillModel skillModel, EffectType effectType)
        {
            var effect = SkillEffectModelFactory.CreateDefaultModel(effectType);
            if (effect != null)
            {
                skillModel.AddEffect(effect);
                EditorUtility.SetDirty(target);
            }
        }
    }
}