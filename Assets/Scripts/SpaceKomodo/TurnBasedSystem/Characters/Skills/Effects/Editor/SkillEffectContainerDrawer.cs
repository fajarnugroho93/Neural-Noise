using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Editor
{
    [CustomPropertyDrawer(typeof(SkillEffectContainer))]
    public class SkillEffectContainerDrawer : PropertyDrawer
    {
        private const float HeaderHeight = 20f;
        private const float SpaceBetweenElements = 2f;
        private bool _foldout = true;
        
        private static EffectRegistry _effectRegistry;
        
        private static EffectRegistry GetEffectRegistry()
        {
            return _effectRegistry ??= EffectEditorUtility.GetEffectRegistry();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var effectTypeProperty = property.FindPropertyRelative("Type");
            if (!_foldout)
                return HeaderHeight;
            
            var effectType = (EffectType)effectTypeProperty.intValue;
            var serializedDataProperty = property.FindPropertyRelative("_serializedData");
            
            var model = EffectEditorUtility.DeserializeEffectModel(effectType, serializedDataProperty.stringValue) 
                     ?? GetEffectRegistry().CreateModel(effectType);
            
            var contentHeight = EditorGUIUtility.singleLineHeight + SpaceBetweenElements;
            contentHeight += EffectEditorUtility.CalculateEffectModelHeight(model, EditorGUIUtility.singleLineHeight, SpaceBetweenElements);
            contentHeight += SpaceBetweenElements * 2;
            
            return HeaderHeight + contentHeight;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var effectTypeProperty = property.FindPropertyRelative("Type");
            var serializedDataProperty = property.FindPropertyRelative("_serializedData");
            
            var headerRect = new Rect(position.x, position.y, position.width, HeaderHeight);
            
            EditorGUI.DrawRect(headerRect, new Color(0.1f, 0.1f, 0.1f, 0.3f));
            
            var foldoutRect = new Rect(headerRect.x + 10, headerRect.y, 20, headerRect.height);
            _foldout = EditorGUI.Foldout(foldoutRect, _foldout, GUIContent.none);
            
            var effectType = (EffectType)effectTypeProperty.intValue;
            var effectTypeRect = new Rect(foldoutRect.x + 15, headerRect.y + 2, 120, EditorGUIUtility.singleLineHeight);
            
            var newEffectType = (EffectType)EditorGUI.EnumPopup(effectTypeRect, effectType);
            
            if (newEffectType != effectType)
            {
                effectTypeProperty.intValue = (int)newEffectType;
                serializedDataProperty.stringValue = "";
                property.serializedObject.ApplyModifiedProperties();
                EditorGUI.EndProperty();
                return;
            }
            
            if (!_foldout)
            {
                EditorGUI.EndProperty();
                return;
            }
            
            var debugRect = new Rect(effectTypeRect.x + 130, headerRect.y + 2, position.width - 160, EditorGUIUtility.singleLineHeight);
            EditorGUI.TextField(debugRect, serializedDataProperty.stringValue);

            var model = EffectEditorUtility.DeserializeEffectModel(effectType, serializedDataProperty.stringValue);
            
            if (model == null)
            {
                model = GetEffectRegistry().CreateModel(effectType);
            }
            
            var contentRect = new Rect(
                position.x, 
                position.y + HeaderHeight, 
                position.width, 
                position.height - HeaderHeight);
            
            var yOffset = 0f;
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var spacing = SpaceBetweenElements;
            
            var targetProperty = model.Target;
            var targetRect = new Rect(contentRect.x, contentRect.y + yOffset, contentRect.width, lineHeight);
            model.Target = (RelativeTarget)EditorGUI.EnumPopup(targetRect, "Target", targetProperty);
            yOffset += lineHeight + spacing;
            
            EffectEditorUtility.DrawEffectModel(
                new Rect(contentRect.x, contentRect.y + yOffset, contentRect.width, contentRect.height - yOffset),
                model, 
                0, 
                lineHeight, 
                spacing);
            
            serializedDataProperty.stringValue = JsonConvert.SerializeObject(model);
            
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
}