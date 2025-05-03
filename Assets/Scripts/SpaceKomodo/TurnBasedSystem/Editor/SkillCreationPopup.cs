using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Editor
{
    public class SkillCreationPopup : EditorWindow
    {
        private const string EnumFilePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/Skill.cs";

        private static TurnBasedEditor _parentEditor;
        private string _skillName = "";
        private string _errorMessage = "";

        public static void Show(TurnBasedEditor parentEditor)
        {
            _parentEditor = parentEditor;
            var window = GetWindow<SkillCreationPopup>(true, "Create New Skill", true);
            window.minSize = new Vector2(400, 200);
            window.maxSize = new Vector2(400, 200);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create New Skill", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            _skillName = EditorGUILayout.TextField("Skill Name:", _skillName);
            
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

                GUI.enabled = !string.IsNullOrEmpty(_skillName);
                if (GUILayout.Button("Create"))
                {
                    if (ValidateSkillName())
                    {
                        CreateSkill();
                        Close();
                    }
                }

                GUI.enabled = true;
            }
        }

        private bool ValidateSkillName()
        {
            _errorMessage = "";

            if (string.IsNullOrWhiteSpace(_skillName))
            {
                _errorMessage = "Skill name cannot be empty.";
                return false;
            }

            if (!Regex.IsMatch(_skillName, @"^[a-zA-Z][a-zA-Z0-9]*$"))
            {
                _errorMessage = "Skill name must start with a letter and contain only letters and numbers.";
                return false;
            }

            var enumValues = Enum.GetNames(typeof(Skill));
            if (enumValues.Contains(_skillName))
            {
                _errorMessage = $"Skill '{_skillName}' already exists.";
                return false;
            }

            return true;
        }

        private void CreateSkill()
        {
            try
            {
                var newValue = GetNextEnumValue();
                UpdateSkillEnum(newValue);

                _parentEditor.AddSkillToEnum((Skill)newValue);

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating skill: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to create skill: {e.Message}", "OK");
            }
        }

        private int GetNextEnumValue()
        {
            var values = Enum.GetValues(typeof(Skill))
                .Cast<int>()
                .OrderBy(v => v)
                .ToList();

            if (!values.Any())
            {
                return 1;
            }

            for (var i = 1; i < int.MaxValue; i++)
            {
                if (!values.Contains(i))
                {
                    return i;
                }
            }

            return values.Max() + 1;
        }

        private void UpdateSkillEnum(int newValue)
        {
            var enumText = File.ReadAllText(EnumFilePath);

            var enumStartIndex = enumText.IndexOf("public enum Skill");
            var enumEndIndex = enumText.IndexOf("}", enumStartIndex);

            var lastEntryIndex = enumText.LastIndexOf(",", enumEndIndex);
            if (lastEntryIndex != -1)
            {
                var newEntry = $"\n        {_skillName} = {newValue},";
                enumText = enumText.Insert(lastEntryIndex + 1, newEntry);

                File.WriteAllText(EnumFilePath, enumText);
                AssetDatabase.ImportAsset(EnumFilePath);
            }
        }
    }
}