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
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var effectTypeProperty = property.FindPropertyRelative("Type");
            if (!_foldout)
                return HeaderHeight;
                
            var effectType = (EffectType)effectTypeProperty.enumValueIndex;
            float lines = 1; 
            
            switch (effectType)
            {
                case EffectType.Damage:
                    lines += 3; 
                    break;
                case EffectType.Heal:
                    lines += 3; 
                    break;
                case EffectType.Shield:
                    lines += 2; 
                    break;
                case EffectType.Poison:
                case EffectType.Burn:
                case EffectType.Stun:
                case EffectType.Blind:
                case EffectType.Silence:
                case EffectType.Root:
                case EffectType.Taunt:
                    lines += 2; 
                    break;
                case EffectType.Energy:
                case EffectType.Rage:
                case EffectType.Mana:
                case EffectType.Focus:
                case EffectType.Charge:
                    lines += 1; 
                    break;
            }
            
            return HeaderHeight + (lines * (EditorGUIUtility.singleLineHeight + SpaceBetweenElements));
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var effectTypeProperty = property.FindPropertyRelative("Type");
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
            
            var debugRect = new Rect(effectTypeRect.x + 130, headerRect.y + 2, position.width - 160, EditorGUIUtility.singleLineHeight);
            EditorGUI.TextField(debugRect, serializedDataProperty.stringValue);

            var model = DeserializeEffectModel(effectType, serializedDataProperty.stringValue) ?? CreateDefaultModel(effectType);

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
                    if (model is DamageEffectModel damageModel)
                    {
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
                    }
                    break;
                    
                case EffectType.Heal:
                    if (model is HealEffectModel healModel)
                    {
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
                    }
                    break;
                    
                case EffectType.Shield:
                    if (model is ShieldEffectModel shieldModel)
                    {
                        shieldModel.Amount = EditorGUI.IntField(
                            new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                            "Amount", shieldModel.Amount);
                        yOffset += lineHeight + spacing;
                        
                        shieldModel.Duration = EditorGUI.IntField(
                            new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                            "Duration", shieldModel.Duration);
                    }
                    break;
                    
                case EffectType.Poison:
                case EffectType.Burn:
                case EffectType.Stun:
                case EffectType.Blind:
                case EffectType.Silence:
                case EffectType.Root:
                case EffectType.Taunt:
                    if (model is IStatusEffect statusModel)
                    {
                        statusModel.Duration = EditorGUI.IntField(
                            new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                            "Duration", statusModel.Duration);
                        yOffset += lineHeight + spacing;
                        
                        statusModel.Amount = EditorGUI.IntField(
                            new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                            "Intensity", statusModel.Amount);
                    }
                    break;
                    
                case EffectType.Energy:
                case EffectType.Rage:
                case EffectType.Mana:
                case EffectType.Focus:
                case EffectType.Charge:
                    if (model is StatusEffectModel resourceModel)
                    {
                        resourceModel.Amount = EditorGUI.IntField(
                            new Rect(contentRect.x, yOffset, contentRect.width, lineHeight), 
                            "Amount", resourceModel.Amount);
                    }
                    break;
            }
            
            serializedDataProperty.stringValue = JsonConvert.SerializeObject(model);
            
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
        
        private IEffectModel DeserializeEffectModel(EffectType type, string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
                return null;
                
            try
            {
                return type switch
                {
                    EffectType.Damage => JsonConvert.DeserializeObject<DamageEffectModel>(serializedData),
                    EffectType.Heal => JsonConvert.DeserializeObject<HealEffectModel>(serializedData),
                    EffectType.Shield => JsonConvert.DeserializeObject<ShieldEffectModel>(serializedData),
                    EffectType.Poison => JsonConvert.DeserializeObject<PoisonEffectModel>(serializedData),
                    EffectType.Burn => JsonConvert.DeserializeObject<BurnEffectModel>(serializedData),
                    EffectType.Stun => JsonConvert.DeserializeObject<StunEffectModel>(serializedData),
                    EffectType.Energy => JsonConvert.DeserializeObject<EnergyEffectModel>(serializedData),
                    EffectType.Rage => JsonConvert.DeserializeObject<RageEffectModel>(serializedData),
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }
        
        private IEffectModel CreateDefaultModel(EffectType type)
        {
            return type switch
            {
                EffectType.Damage => new DamageEffectModel { Amount = 10, CriticalChance = 0.1f, CriticalMultiplier = 1.5f },
                EffectType.Heal => new HealEffectModel { Amount = 15, CriticalChance = 0.1f, CriticalMultiplier = 1.5f },
                EffectType.Shield => new ShieldEffectModel { Amount = 10, Duration = 3 },
                EffectType.Poison => new PoisonEffectModel { Duration = 3, Amount = 5 },
                EffectType.Burn => new BurnEffectModel { Duration = 3, Amount = 7 },
                EffectType.Stun => new StunEffectModel { Duration = 1, Amount = 1 },
                EffectType.Energy => new EnergyEffectModel { Amount = 10 },
                EffectType.Rage => new RageEffectModel { Amount = 5 },
                EffectType.Mana => new EnergyEffectModel { Amount = 10 },
                EffectType.Focus => new EnergyEffectModel { Amount = 10 },
                EffectType.Charge => new EnergyEffectModel { Amount = 10 },
                _ => new DamageEffectModel()
            };
        }
    }
}