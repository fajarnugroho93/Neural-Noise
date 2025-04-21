using System;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Editor
{
    [CustomPropertyDrawer(typeof(SkillEffectModelContainer))]
    public class SkillEffectModelDrawer : PropertyDrawer
    {
        private const float HeaderHeight = 20f;
        private const float SpaceBetweenElements = 2f;
        private bool _foldout = true;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var effectTypeProperty = property.FindPropertyRelative("EffectType");
            if (!_foldout)
                return HeaderHeight;
                
            var effectType = (EffectType)effectTypeProperty.enumValueIndex;
            float lines = 1; // For Target
            
            switch (effectType)
            {
                case EffectType.Damage:
                    lines += 4; // Amount, CriticalChance, CriticalMultiplier, DamageType
                    break;
                case EffectType.Heal:
                    lines += 3; // Amount, CriticalChance, CriticalMultiplier
                    break;
                case EffectType.Shield:
                    lines += 2; // Amount, Duration
                    break;
                case EffectType.Status:
                    lines += 3; // StatusType, Duration, Intensity
                    break;
                case EffectType.Resource:
                    lines += 2; // ResourceType, Amount
                    break;
            }
            
            return HeaderHeight + (lines * (EditorGUIUtility.singleLineHeight + SpaceBetweenElements));
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var effectTypeProperty = property.FindPropertyRelative("EffectType");
            var serializedDataProperty = property.FindPropertyRelative("_serializedData");
            
            var headerRect = new Rect(position.x, position.y, position.width, HeaderHeight);
            
            EditorGUI.DrawRect(new Rect(headerRect.x, headerRect.y, headerRect.width, headerRect.height), 
                new Color(0.1f, 0.1f, 0.1f, 0.3f));
            
            var foldoutRect = new Rect(headerRect.x + 10, headerRect.y, 20, headerRect.height);
            _foldout = EditorGUI.Foldout(foldoutRect, _foldout, GUIContent.none);
            
            var effectType = (EffectType)effectTypeProperty.enumValueIndex;
            var effectTypeRect = new Rect(foldoutRect.x + 15, headerRect.y + 2, 120, EditorGUIUtility.singleLineHeight);
            var newEffectType = (EffectType)EditorGUI.EnumPopup(effectTypeRect, effectType);
            
            if (newEffectType != effectType)
            {
                effectTypeProperty.enumValueIndex = (int)newEffectType;
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
            
            BaseSkillEffectModel model = null;
            if (!string.IsNullOrEmpty(serializedDataProperty.stringValue))
            {
                try
                {
                    switch (effectType)
                    {
                        case EffectType.Damage:
                            model = JsonUtility.FromJson<DamageEffectModel>(serializedDataProperty.stringValue);
                            break;
                        case EffectType.Heal:
                            model = JsonUtility.FromJson<HealEffectModel>(serializedDataProperty.stringValue);
                            break;
                        case EffectType.Shield:
                            model = JsonUtility.FromJson<ShieldEffectModel>(serializedDataProperty.stringValue);
                            break;
                        case EffectType.Status:
                            model = JsonUtility.FromJson<StatusEffectModel>(serializedDataProperty.stringValue);
                            break;
                        case EffectType.Resource:
                            model = JsonUtility.FromJson<ResourceEffectModel>(serializedDataProperty.stringValue);
                            break;
                    }
                }
                catch (Exception)
                {
                    model = null;
                }
            }
            
            if (model == null)
            {
                model = SkillEffectModelFactory.CreateDefaultModel(effectType);
            }
            
            var contentRect = new Rect(position.x, position.y + HeaderHeight, position.width, 
                position.height - HeaderHeight);
            
            var yOffset = contentRect.y;
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var spacing = SpaceBetweenElements;
            
            var targetRect = new Rect(contentRect.x, yOffset, contentRect.width, lineHeight);
            model.Target = (RelativeTarget)EditorGUI.EnumPopup(targetRect, "Target", model.Target);
            yOffset += lineHeight + spacing;
            
            switch (effectType)
            {
                case EffectType.Damage:
                    var damageModel = (DamageEffectModel)model;
                    
                    damageModel.Amount = EditorGUI.IntField(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Amount", damageModel.Amount);
                    yOffset += lineHeight + spacing;
                    
                    damageModel.CriticalChance = EditorGUI.Slider(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Critical Chance", damageModel.CriticalChance, 0f, 1f);
                    yOffset += lineHeight + spacing;
                    
                    damageModel.CriticalMultiplier = EditorGUI.Slider(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Critical Multiplier", damageModel.CriticalMultiplier, 1f, 3f);
                    yOffset += lineHeight + spacing;
                    
                    damageModel.DamageType = (DamageType)EditorGUI.EnumPopup(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Damage Type", damageModel.DamageType);
                    break;
                    
                case EffectType.Heal:
                    var healModel = (HealEffectModel)model;
                    
                    healModel.Amount = EditorGUI.IntField(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Amount", healModel.Amount);
                    yOffset += lineHeight + spacing;
                    
                    healModel.CriticalChance = EditorGUI.Slider(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Critical Chance", healModel.CriticalChance, 0f, 1f);
                    yOffset += lineHeight + spacing;
                    
                    healModel.CriticalMultiplier = EditorGUI.Slider(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Critical Multiplier", healModel.CriticalMultiplier, 1f, 3f);
                    break;
                    
                case EffectType.Shield:
                    var shieldModel = (ShieldEffectModel)model;
                    
                    shieldModel.Amount = EditorGUI.IntField(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Amount", shieldModel.Amount);
                    yOffset += lineHeight + spacing;
                    
                    shieldModel.Duration = EditorGUI.IntField(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Duration", shieldModel.Duration);
                    break;
                    
                case EffectType.Status:
                    var statusModel = (StatusEffectModel)model;
                    
                    statusModel.StatusType = (StatusType)EditorGUI.EnumPopup(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Status Type", statusModel.StatusType);
                    yOffset += lineHeight + spacing;
                    
                    statusModel.Duration = EditorGUI.IntField(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Duration", statusModel.Duration);
                    yOffset += lineHeight + spacing;
                    
                    statusModel.Intensity = EditorGUI.IntField(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Intensity", statusModel.Intensity);
                    break;
                    
                case EffectType.Resource:
                    var resourceModel = (ResourceEffectModel)model;
                    
                    resourceModel.ResourceType = (ResourceType)EditorGUI.EnumPopup(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Resource Type", resourceModel.ResourceType);
                    yOffset += lineHeight + spacing;
                    
                    resourceModel.Amount = EditorGUI.IntField(
                        new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                        "Amount", resourceModel.Amount);
                    break;
            }
            
            serializedDataProperty.stringValue = JsonUtility.ToJson(model);
            
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
}