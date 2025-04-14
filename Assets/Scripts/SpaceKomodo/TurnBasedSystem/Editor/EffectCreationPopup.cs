using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Editor
{
    public class EffectCreationPopup : EditorWindow
    {
        private const string EnumFilePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/SkillEffect.cs";

        private static TurnBasedEditor _parentEditor;
        private string _effectName = "";
        private string _errorMessage = "";

        public static void Show(TurnBasedEditor parentEditor)
        {
            _parentEditor = parentEditor;
            var window = GetWindow<EffectCreationPopup>(true, "Create New Effect", true);
            window.minSize = new Vector2(400, 180);
            window.maxSize = new Vector2(400, 180);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create New Effect", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            _effectName = EditorGUILayout.TextField("Effect Name:", _effectName);

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

            var enumValues = Enum.GetNames(typeof(SkillEffect));
            if (enumValues.Contains(_effectName))
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
                var newValue = GetNextEnumValue();
                UpdateEffectEnum(newValue);

                _parentEditor.AddEffectToEnum((SkillEffect)newValue);

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating effect: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to create effect: {e.Message}", "OK");
            }
        }

        private int GetNextEnumValue()
        {
            var values = Enum.GetValues(typeof(SkillEffect))
                .Cast<int>()
                .OrderBy(v => v)
                .ToList();

            if (!values.Any())
            {
                return 1;
            }

            for (int i = 1; i < int.MaxValue; i++)
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

            var enumStartIndex = enumText.IndexOf("public enum SkillEffect");
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
    }
}