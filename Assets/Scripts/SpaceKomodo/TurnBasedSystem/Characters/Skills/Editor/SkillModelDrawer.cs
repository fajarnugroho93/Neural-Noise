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
            
            _effectRegistry = new EffectRegistry(damageCalculator, statusEffectManager, resourceManager);
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
                        
                        AddEffectMenuItem(menu, "Damage", EffectType.Damage, skillModel);
                        AddEffectMenuItem(menu, "Heal", EffectType.Heal, skillModel);
                        AddEffectMenuItem(menu, "Shield", EffectType.Shield, skillModel);
                        
                        menu.AddSeparator("");
                        
                        AddEffectMenuItem(menu, "Status/Poison", EffectType.Poison, skillModel);
                        AddEffectMenuItem(menu, "Status/Burn", EffectType.Burn, skillModel);
                        AddEffectMenuItem(menu, "Status/Stun", EffectType.Stun, skillModel);
                        AddEffectMenuItem(menu, "Status/Blind", EffectType.Blind, skillModel);
                        AddEffectMenuItem(menu, "Status/Silence", EffectType.Silence, skillModel);
                        AddEffectMenuItem(menu, "Status/Root", EffectType.Root, skillModel);
                        AddEffectMenuItem(menu, "Status/Taunt", EffectType.Taunt, skillModel);
                        
                        menu.AddSeparator("");
                        
                        AddEffectMenuItem(menu, "Resource/Energy", EffectType.Energy, skillModel);
                        AddEffectMenuItem(menu, "Resource/Rage", EffectType.Rage, skillModel);
                        AddEffectMenuItem(menu, "Resource/Mana", EffectType.Mana, skillModel);
                        AddEffectMenuItem(menu, "Resource/Focus", EffectType.Focus, skillModel);
                        AddEffectMenuItem(menu, "Resource/Charge", EffectType.Charge, skillModel);
                        
                        menu.ShowAsContext();
                    }
                }
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