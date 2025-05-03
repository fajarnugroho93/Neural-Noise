using SpaceKomodo.TurnBasedSystem.Effects;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Core.Editor
{
    [CustomEditor(typeof(EffectRegistriesScriptableObject))]
    public class EffectRegistriesEditor : UnityEditor.Editor
    {
        private SerializedProperty _basicEffectsProperty;
        private SerializedProperty _statusEffectsProperty;
        private SerializedProperty _resourceEffectsProperty;
        
        private void OnEnable()
        {
            _basicEffectsProperty = serializedObject.FindProperty("BasicEffects");
            _statusEffectsProperty = serializedObject.FindProperty("StatusEffects");
            _resourceEffectsProperty = serializedObject.FindProperty("ResourceEffects");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_basicEffectsProperty, new GUIContent("Basic Effects"), true);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(_statusEffectsProperty, new GUIContent("Status Effects"), true);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(_resourceEffectsProperty, new GUIContent("Resource Effects"), true);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Fetch All Effect Assets"))
            {
                var registries = target as EffectRegistriesScriptableObject;
                if (registries != null)
                {
                    registries.DoFetchAssets();
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}