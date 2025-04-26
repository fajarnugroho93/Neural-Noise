using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Editor
{
    public class EffectCreationPopup : EditorWindow
    {
        private const string EnumFilePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/Effects/EffectInterfaces.cs";
        private const string EffectModelPath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/Effects/Models";
        
        private static TurnBasedEditor _parentEditor;
        private string _effectName = "";
        private EffectCategory _category = EffectCategory.Status;
        private int _categoryIndex = 0;
        private string _errorMessage = "";

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
                Directory.CreateDirectory(EffectModelPath);
                
                var newValue = GetNextEnumValue();
                UpdateEffectEnum(newValue);
                CreateEffectModelClass();
                
                AssetDatabase.Refresh();
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
            var values = Enum.GetValues(typeof(EffectType))
                .Cast<int>()
                .OrderBy(v => v)
                .ToList();

            if (!values.Any())
            {
                return 0;
            }

            return values.Max() + 1;
        }

        private void UpdateEffectEnum(int newValue)
        {
            var enumText = File.ReadAllText(EnumFilePath);

            var enumStartIndex = enumText.IndexOf("public enum EffectType");
            var enumEndIndex = enumText.IndexOf("}", enumStartIndex);

            var lastEntryIndex = enumText.LastIndexOf(",", enumEndIndex);
            if (lastEntryIndex != -1)
            {
                var newEntry = $"\n        {_effectName} = {newValue},";
                enumText = enumText.Insert(lastEntryIndex + 1, newEntry);

                File.WriteAllText(EnumFilePath, enumText);
                AssetDatabase.ImportAsset(EnumFilePath);
            }
        }
        
        private void CreateEffectModelClass()
        {
            var className = $"{_effectName}EffectModel";
            var filePath = $"{EffectModelPath}/{className}.cs";
            
            var template = GetModelTemplate(className);
            File.WriteAllText(filePath, template);
            
            AssetDatabase.ImportAsset(filePath);
        }
        
        private string GetModelTemplate(string className)
        {
            string baseClass = GetBaseClassForCategory(_category);
            
            return $@"using System;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{{
    [Serializable]
    public class {className} : {baseClass}
    {{
        public override EffectType Type => EffectType.{_effectName};
    }}
}}";
        }

        private string GetBaseClassForCategory(EffectCategory category)
        {
            return category switch
            {
                EffectCategory.Basic => "InstantEffectModel",
                EffectCategory.Status => "StatusEffectModel",
                EffectCategory.Resource => "StatusEffectModel",
                _ => "StatusEffectModel"
            };
        }
    }
}