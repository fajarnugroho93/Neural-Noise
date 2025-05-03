using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Editor
{
    public class CharacterCreationPopup : EditorWindow
    {
        private const string CharacterFolderPath = "Assets/Resources/Data/Characters/";
        private const string EnumFilePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Character.cs";
        
        private static TurnBasedEditor _parentEditor;
        private CharacterGroup _characterGroup = CharacterGroup.Hero;
        private string _characterName = "";
        private string _errorMessage = "";
        
        public static void Show(TurnBasedEditor parentEditor)
        {
            _parentEditor = parentEditor;
            var window = GetWindow<CharacterCreationPopup>(true, "Create New Character", true);
            window.minSize = new Vector2(400, 200);
            window.maxSize = new Vector2(400, 200);
            window.ShowUtility();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create New Character", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            _characterGroup = (CharacterGroup)EditorGUILayout.EnumPopup("Character Group:", _characterGroup);
            _characterName = EditorGUILayout.TextField("Character Name:", _characterName);
            
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
                
                GUI.enabled = !string.IsNullOrEmpty(_characterName);
                if (GUILayout.Button("Create"))
                {
                    if (ValidateCharacterName())
                    {
                        CreateCharacter();
                        Close();
                    }
                }
                GUI.enabled = true;
            }
        }
        
        private bool ValidateCharacterName()
        {
            _errorMessage = "";
            
            if (string.IsNullOrWhiteSpace(_characterName))
            {
                _errorMessage = "Character name cannot be empty.";
                return false;
            }
            
            if (!Regex.IsMatch(_characterName, @"^[a-zA-Z][a-zA-Z0-9]*$"))
            {
                _errorMessage = "Character name must start with a letter and contain only letters and numbers.";
                return false;
            }
            
            var enumValues = Enum.GetNames(typeof(Character));
            if (enumValues.Contains(_characterName))
            {
                _errorMessage = $"Character '{_characterName}' already exists.";
                return false;
            }
            
            return true;
        }
        
        private void CreateCharacter()
        {
            try
            {
                Directory.CreateDirectory(CharacterFolderPath);
                
                var newValue = GetNextEnumValue();
                UpdateCharacterEnum(newValue);
                
                var character = CreateCharacterAsset(newValue);
                _parentEditor.AddCharacterToList(character);
                
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating character: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to create character: {e.Message}", "OK");
            }
        }
        
        private int GetNextEnumValue()
        {
            var baseValue = (int)_characterGroup;
            var maxValue = baseValue + 9999;
    
            var values = Enum.GetValues(typeof(Character))
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
        
        private void UpdateCharacterEnum(int newValue)
        {
            var enumText = File.ReadAllText(EnumFilePath);
            
            var enumStartIndex = enumText.IndexOf("public enum Character");
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
                var newEntry = $"\n        {_characterName} = {newValue},";
                enumText = enumText.Insert(insertPoint, newEntry);
                
                File.WriteAllText(EnumFilePath, enumText);
                AssetDatabase.ImportAsset(EnumFilePath);
            }
        }
        
        private string GetGroupCommentLine()
        {
            return $"// {_characterGroup} = {(int)_characterGroup}";
        }
        
        private CharacterScriptableObject CreateCharacterAsset(int enumValue)
        {
            var newCharacter = CreateInstance<CharacterScriptableObject>();
            
            newCharacter.CharacterModel = new CharacterModel
            {
                CharacterGroup = _characterGroup,
                Character = (Character)enumValue,
                Health = 100,
                Speed = 5,
                Skills = new System.Collections.Generic.List<SkillModel>()
            };
            
            var assetPath = $"{CharacterFolderPath}{enumValue}-{_characterName}.asset";
            AssetDatabase.CreateAsset(newCharacter, assetPath);
            AssetDatabase.SaveAssets();
            
            return newCharacter;
        }
    }
}