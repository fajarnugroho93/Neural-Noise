using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using SpaceKomodo.TurnBasedSystem.Effects;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Core.Editor
{
    [CustomEditor(typeof(EffectRegistryScriptableObject))]
    public class EffectRegistryEditor : UnityEditor.Editor
    {
        private SerializedProperty _categoryProperty;
        private SerializedProperty _effectTypeProperty;
        private SerializedProperty _defaultAmountProperty;
        private SerializedProperty _defaultDurationProperty;
        private SerializedProperty _defaultCriticalChanceProperty;
        private SerializedProperty _defaultCriticalMultiplierProperty;
        
        private void OnEnable()
        {
            _categoryProperty = serializedObject.FindProperty("Category");
            _effectTypeProperty = serializedObject.FindProperty("EffectType");
            _defaultAmountProperty = serializedObject.FindProperty("_defaultAmount");
            _defaultDurationProperty = serializedObject.FindProperty("_defaultDuration");
            _defaultCriticalChanceProperty = serializedObject.FindProperty("_defaultCriticalChance");
            _defaultCriticalMultiplierProperty = serializedObject.FindProperty("_defaultCriticalMultiplier");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_categoryProperty);
            EditorGUILayout.PropertyField(_effectTypeProperty);
            // EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Default Values", EditorStyles.boldLabel);
            
            var category = (EffectCategory)_categoryProperty.intValue;
            
            switch (category)
            {
                case EffectCategory.Basic:
                    EditorGUILayout.PropertyField(_defaultAmountProperty, new GUIContent("Default Amount"));
                    EditorGUILayout.PropertyField(_defaultCriticalChanceProperty, new GUIContent("Default Critical Chance"));
                    EditorGUILayout.PropertyField(_defaultCriticalMultiplierProperty, new GUIContent("Default Critical Multiplier"));
                    break;
                    
                case EffectCategory.Status:
                    EditorGUILayout.PropertyField(_defaultAmountProperty, new GUIContent("Default Intensity"));
                    EditorGUILayout.PropertyField(_defaultDurationProperty, new GUIContent("Default Duration"));
                    break;
                    
                case EffectCategory.Resource:
                    EditorGUILayout.PropertyField(_defaultAmountProperty, new GUIContent("Default Amount"));
                    break;
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Generate Model Class"))
            {
                var registry = target as EffectRegistryScriptableObject;
                if (registry != null)
                {
                    GenerateModelClass(registry);
                }
            }
            
            if (GUILayout.Button("Generate Behavior Class"))
            {
                var registry = target as EffectRegistryScriptableObject;
                if (registry != null && registry.Category != EffectCategory.Status)
                {
                    GenerateBehaviorClass(registry);
                }
            }
            
            serializedObject.ApplyModifiedProperties();
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
        
        private void GenerateModelClass(EffectRegistryScriptableObject registry)
        {
            var className = registry.ModelClassName;
            var baseTypeName = registry.BaseModelTypeName;
            var effectName = registry.EffectTypeName;
            var filePath = $"Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/Effects/Models/{className}.cs";
            
            var template = $@"using System;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{{
    [Serializable]
    public class {className} : {baseTypeName}
    {{
        public override EffectType Type => EffectType.{effectName};
    }}
}}";
            
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            System.IO.File.WriteAllText(filePath, template);
            AssetDatabase.ImportAsset(filePath);
            
            EditorUtility.DisplayDialog("Success", $"Model class {className} generated successfully!", "OK");
        }
        
        private void GenerateBehaviorClass(EffectRegistryScriptableObject registry)
        {
            var className = registry.BehaviorClassName;
            var filePath = $"Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/Effects/Behaviors/{className}.cs";
            
            var behaviorInterface = "IEffectBehavior";
            var constructorParams = "";
            var constructorParamsClass = "";
            var executionLogic = "";
            
            switch (registry.Category)
            {
                case EffectCategory.Basic:
                    constructorParams = "DamageCalculator damageCalculator";
                    constructorParamsClass = "_damageCalculator";
                    executionLogic = @"private readonly DamageCalculator _damageCalculator;
        
        public " + className + @"(DamageCalculator damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }

        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is IInstantEffect model))
                return;

            var isCritical = UnityEngine.Random.value < model.CriticalChance;
            var finalAmount = model.Amount;
            
            if (isCritical)
            {
                finalAmount = Mathf.RoundToInt(finalAmount * model.CriticalMultiplier);
            }
            
            // Implement your effect logic here
        }

        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is IInstantEffect model))
                return result;

            var normalAmount = model.Amount;
            var criticalAmount = Mathf.RoundToInt(model.Amount * model.CriticalMultiplier);

            result[""MinAmount""] = normalAmount;
            result[""MaxAmount""] = criticalAmount;
            result[""CriticalChance""] = model.CriticalChance;

            return result;
        }";
                    break;
                    
                case EffectCategory.Resource:
                    constructorParams = "ResourceManager resourceManager";
                    constructorParamsClass = "_resourceManager";
                    executionLogic = @"private readonly ResourceManager _resourceManager;
        
        public " + className + @"(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }
        
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is IAmountEffect model))
                return;
            
            var amount = model.Amount;
            var resourceType = (int)effectModel.Type;
            
            if (amount > 0)
            {
                _resourceManager.AddResource(target, resourceType, amount);
            }
            else if (amount < 0)
            {
                _resourceManager.ConsumeResource(target, resourceType, -amount);
            }
        }
        
        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is IAmountEffect model))
                return result;
            
            result[""ResourceType""] = effectModel.Type;
            result[""Amount""] = model.Amount;
            
            return result;
        }";
                    break;
            }
            
            var template = $@"using System.Collections.Generic;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Effects;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{{
    public class {className} : {behaviorInterface}
    {{
        {executionLogic}
    }}
}}";
            
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            System.IO.File.WriteAllText(filePath, template);
            AssetDatabase.ImportAsset(filePath);
            
            EditorUtility.DisplayDialog("Success", $"Behavior class {className} generated successfully!", "OK");
        }
    }
}