using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using SpaceKomodo.TurnBasedSystem.Effects;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Editor
{
    public class EffectCreationPopup : EditorWindow
    {
        private const string EnumFilePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/Effects/EffectInterfaces.cs";
        private const string EffectFolderPath = "Assets/Resources/Data/Effects/";
        private const string ModelTemplatePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/Effects/Models/";
        private const string BehaviorTemplatePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/Effects/Behaviors/";
        
        private static TurnBasedEditor _parentEditor;
        private string _effectName = "";
        private EffectCategory _category = EffectCategory.Status;
        private int _categoryIndex = 0;
        private string _errorMessage = "";
        private string _implementationClassName = "";

        public static void Show(TurnBasedEditor parentEditor)
        {
            _parentEditor = parentEditor;
            var window = GetWindow<EffectCreationPopup>(true, "Create New Effect", true);
            window.minSize = new Vector2(400, 250);
            window.maxSize = new Vector2(400, 250);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create New Effect", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            _effectName = EditorGUILayout.TextField("Effect Name:", _effectName);
            
            _categoryIndex = EditorGUILayout.Popup("Category:", _categoryIndex, Enum.GetNames(typeof(EffectCategory)));
            _category = (EffectCategory)_categoryIndex;
            
            _implementationClassName = EditorGUILayout.TextField("Implementation Class:", string.IsNullOrEmpty(_implementationClassName) ? $"{_effectName}Behavior" : _implementationClassName);
            
            EditorGUILayout.Space(10);

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                EditorGUILayout.HelpBox(_errorMessage, MessageType.Error);
            }

            EditorGUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }

                GUI.enabled = !string.IsNullOrEmpty(_effectName);
                if (GUILayout.Button("Create"))
                {
                    if (ValidateEffectName())
                    {
                        CreateEffect();
                        Close();
                    }
                }

                GUI.enabled = true;
            }
        }

        private bool ValidateEffectName()
        {
            _errorMessage = "";

            if (string.IsNullOrWhiteSpace(_effectName))
            {
                _errorMessage = "Effect name cannot be empty.";
                return false;
            }

            if (!Regex.IsMatch(_effectName, @"^[a-zA-Z][a-zA-Z0-9]*$"))
            {
                _errorMessage = "Effect name must start with a letter and contain only letters and numbers.";
                return false;
            }
            
            var existingValues = Enum.GetNames(typeof(EffectType));
            if (existingValues.Contains(_effectName))
            {
                _errorMessage = $"Effect '{_effectName}' already exists.";
                return false;
            }

            return true;
        }

        private void CreateEffect()
        {
            try
            {
                Directory.CreateDirectory(EffectFolderPath);
                Directory.CreateDirectory(ModelTemplatePath);
                Directory.CreateDirectory(BehaviorTemplatePath);
                
                var newValue = GetNextEnumValue();
                UpdateEffectEnum(newValue);
                
                var registryAsset = CreateEffectRegistry(newValue);
                GenerateModelClass(registryAsset);
                GenerateBehaviorClass(registryAsset);
                
                AssetDatabase.Refresh();
                
                _parentEditor?.AddEffectToList(registryAsset);
                
                EditorUtility.DisplayDialog("Success", $"Effect '{_effectName}' created successfully!", "OK");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating effect: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to create effect: {e.Message}", "OK");
            }
        }
        
        private int GetNextEnumValue()
        {
            var baseValue = (int)_category;
            var maxValue = baseValue + 9999;

            var values = Enum.GetValues(typeof(EffectType))
                .Cast<int>()
                .Where(v => v >= baseValue && v < maxValue)
                .OrderBy(v => v)
                .ToList();

            if (!values.Any())
            {
                return baseValue;
            }

            for (var i = baseValue; i < maxValue; i++)
            {
                if (!values.Contains(i))
                {
                    return i;
                }
            }

            return values.Max() + 1;
        }

        private void UpdateEffectEnum(int newValue)
        {
            var enumText = File.ReadAllText(EnumFilePath);
            
            var enumStartIndex = enumText.IndexOf("public enum EffectType");
            var enumEndIndex = enumText.IndexOf("}", enumStartIndex);
            
            var enumContent = enumText.Substring(enumStartIndex, enumEndIndex - enumStartIndex);
            
            var groupComment = GetGroupCommentLine();
            var insertPoint = -1;
            
            if (enumContent.Contains(groupComment))
            {
                var groupCommentIndex = enumContent.IndexOf(groupComment);
                
                var nextGroupIndex = enumContent.IndexOf("// ", groupCommentIndex + groupComment.Length);
                if (nextGroupIndex == -1)
                {
                    nextGroupIndex = enumContent.IndexOf("}", groupCommentIndex);
                }
                
                var lastEnumInGroup = enumContent.Substring(groupCommentIndex, nextGroupIndex - groupCommentIndex)
                    .Split('\n')
                    .LastOrDefault(line => line.Contains("=") && line.Contains(","));
                
                if (lastEnumInGroup != null)
                {
                    insertPoint = enumStartIndex + groupCommentIndex + enumContent.Substring(groupCommentIndex).IndexOf(lastEnumInGroup) + lastEnumInGroup.Length;
                }
            }
            else
            {
                var lastGroupIndex = enumContent.LastIndexOf("// ");
                if (lastGroupIndex != -1)
                {
                    var lastGroup = enumContent.Substring(lastGroupIndex);
                    var lastEnumInLastGroup = lastGroup.Split('\n')
                        .LastOrDefault(line => line.Contains("=") && line.Contains(","));
                        
                    if (lastEnumInLastGroup != null)
                    {
                        insertPoint = enumStartIndex + lastGroupIndex + lastGroup.IndexOf(lastEnumInLastGroup) + lastEnumInLastGroup.Length;
                        
                        var newEntry = $"\n\n        {groupComment}";
                        enumText = enumText.Insert(insertPoint, newEntry);
                        insertPoint += newEntry.Length;
                    }
                }
                else
                {
                    insertPoint = enumEndIndex - 1;
                    
                    var newEntry = $"\n        {groupComment}";
                    enumText = enumText.Insert(insertPoint, newEntry);
                    insertPoint += newEntry.Length;
                }
            }
            
            if (insertPoint != -1)
            {
                var newEntry = $"\n        {_effectName} = {newValue},";
                enumText = enumText.Insert(insertPoint, newEntry);
                
                File.WriteAllText(EnumFilePath, enumText);
                AssetDatabase.ImportAsset(EnumFilePath);
            }
        }

        private string GetGroupCommentLine()
        {
            return $"// {_category} = {(int)_category}";
        }
        
        private EffectRegistryScriptableObject CreateEffectRegistry(int enumValue)
        {
            var registry = CreateInstance<EffectRegistryScriptableObject>();
            
            registry.EffectType = (EffectType)enumValue;
            registry.Category = _category;
            registry.ImplementationClassName = _implementationClassName;
            registry.SetBaseModelType();
            
            var assetPath = $"{EffectFolderPath}{enumValue}-{_effectName}.asset";
            AssetDatabase.CreateAsset(registry, assetPath);
            AssetDatabase.SaveAssets();
            
            return registry;
        }
        
        private void GenerateModelClass(EffectRegistryScriptableObject registry)
        {
            var className = registry.GetModelClassName();
            var baseTypeName = registry.GetBaseModelTypeName();
            var filePath = $"{ModelTemplatePath}{className}.cs";
            
            var template = $@"using System;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{{
    [Serializable]
    public class {className} : {baseTypeName}
    {{
        public override EffectType Type => EffectType.{_effectName};
    }}
}}";
            
            File.WriteAllText(filePath, template);
            AssetDatabase.ImportAsset(filePath);
        }
        
        private void GenerateBehaviorClass(EffectRegistryScriptableObject registry)
        {
            if (string.IsNullOrEmpty(_implementationClassName) || _category == EffectCategory.Status)
                return;
                
            var className = registry.GetBehaviorClassName();
            var filePath = $"{BehaviorTemplatePath}{className}.cs";
            
            var behaviorInterface = "IEffectBehavior";
            var constructorParams = "";
            var constructorParamsClass = "";
            var executionLogic = "";
            
            switch (_category)
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

            var isCritical = Random.value < model.CriticalChance;
            var finalAmount = model.Amount;
            
            if (isCritical)
            {
                finalAmount = Mathf.RoundToInt(finalAmount * model.CriticalMultiplier);
            }
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
            
            File.WriteAllText(filePath, template);
            AssetDatabase.ImportAsset(filePath);
        }
    }
}