using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TurnBasedSystem.Characters;
using TurnBasedSystem.Characters.Skills;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TurnBasedSystem.Editor
{
    public class CharactersEditor : EditorWindow
    {
        private const string WindowTitle = "Characters Editor";
        private const string EnumFilePath = "Assets/Scripts/TurnBasedSystem/Characters/Character.cs";
        private readonly List<Character> _protectedCharacters = new() 
        { 
            Character.None, 
            Character.Warrior, 
            Character.Mossball, 
            Character.Mossking 
        };
        
        private CharactersScriptableObject _charactersScriptableObject;
        private List<CharacterScriptableObject> _currentCharacterList = new();
        private List<CharacterScriptableObject> _filteredCharacterList = new();
        
        private VisualElement _mainContainer;
        private VisualElement _listContainer;
        private VisualElement _detailsContainer;
        private VisualElement _toolbarContainer;
        
        private TextField _searchField;
        private DropdownField _sortDropdown;
        private ListView _characterListView;
        private ScrollView _characterDetailsView;
        private CharacterScriptableObject _selectedCharacter;
        
        private Label _noSelectionLabel;
        private Button _addButton;
        private Button _deleteButton;
        
        private ToolbarToggle _heroesToggle;
        private ToolbarToggle _enemiesToggle;
        private ToolbarToggle _bossesToggle;
        private int _currentTabIndex = 0;
        
        private CharacterGroup _currentCharacterGroup = CharacterGroup.Hero;

        [MenuItem("Tools/TurnBasedSystem/Characters Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<CharactersEditor>();
            window.titleContent = new GUIContent(WindowTitle);
            window.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            LoadUIElements();
            LoadDataSource();
            RefreshCharacterList();
            SetupSortDropdown();
        }

        private void LoadUIElements()
        {
            var root = rootVisualElement;
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/TurnBasedSystem/Editor/CharactersEditor.uss");
            if (styleSheet != null)
            {
                root.styleSheets.Add(styleSheet);
            }
            
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/TurnBasedSystem/Editor/CharactersEditor.uxml");
            if (visualTree != null)
            {
                visualTree.CloneTree(root);
            }
            
            SetupUIReferences();
            RegisterCallbacks();
        }

        private void SetupUIReferences()
        {
            _mainContainer = rootVisualElement.Q<VisualElement>("MainContainer");
            _toolbarContainer = rootVisualElement.Q<VisualElement>("ToolbarContainer");
            _listContainer = rootVisualElement.Q<VisualElement>("ListContainer");
            _detailsContainer = rootVisualElement.Q<VisualElement>("DetailsContainer");
            
            _heroesToggle = rootVisualElement.Q<ToolbarToggle>("HeroesToggle");
            _enemiesToggle = rootVisualElement.Q<ToolbarToggle>("EnemiesToggle");
            _bossesToggle = rootVisualElement.Q<ToolbarToggle>("BossesToggle") ?? CreateBossesToggle();
            
            _searchField = rootVisualElement.Q<TextField>("SearchField");
            _sortDropdown = rootVisualElement.Q<DropdownField>("SortDropdown");
            
            _characterListView = rootVisualElement.Q<ListView>("CharacterListView");
            _noSelectionLabel = rootVisualElement.Q<Label>("NoSelectionLabel");
            _characterDetailsView = rootVisualElement.Q<ScrollView>("CharacterDetailsView");
            
            _addButton = rootVisualElement.Q<Button>("AddButton");
            _deleteButton = rootVisualElement.Q<Button>("DeleteButton");
            
            SetupListView();
        }
        
        private ToolbarToggle CreateBossesToggle()
        {
            var toggle = new ToolbarToggle();
            toggle.name = "BossesToggle";
            toggle.text = "Bosses";
            toggle.value = false;
            
            var toggleContainer = _heroesToggle.parent;
            toggleContainer.Add(toggle);
            
            return toggle;
        }

        private void RegisterCallbacks()
        {
            _heroesToggle.RegisterValueChangedCallback(evt => 
            {
                if (evt.newValue)
                {
                    _enemiesToggle.SetValueWithoutNotify(false);
                    _bossesToggle.SetValueWithoutNotify(false);
                    _currentCharacterGroup = CharacterGroup.Hero;
                    RefreshCharacterList();
                    _selectedCharacter = null;
                    ShowNoSelectionMessage();
                }
            });
            
            _enemiesToggle.RegisterValueChangedCallback(evt => 
            {
                if (evt.newValue)
                {
                    _heroesToggle.SetValueWithoutNotify(false);
                    _bossesToggle.SetValueWithoutNotify(false);
                    _currentCharacterGroup = CharacterGroup.Enemy;
                    RefreshCharacterList();
                    _selectedCharacter = null;
                    ShowNoSelectionMessage();
                }
            });
            
            _bossesToggle.RegisterValueChangedCallback(evt => 
            {
                if (evt.newValue)
                {
                    _heroesToggle.SetValueWithoutNotify(false);
                    _enemiesToggle.SetValueWithoutNotify(false);
                    _currentCharacterGroup = CharacterGroup.Boss;
                    RefreshCharacterList();
                    _selectedCharacter = null;
                    ShowNoSelectionMessage();
                }
            });
            
            _searchField.RegisterValueChangedCallback(evt => FilterCharacterList(evt.newValue));
            
            _sortDropdown.RegisterValueChangedCallback(evt => SortCharacterList());
            
            _addButton.clicked += OnAddButtonClicked;
            _deleteButton.clicked += OnDeleteButtonClicked;
        }

        private void SetupSortDropdown()
        {
            _sortDropdown.choices = new List<string> { "Name", "Health", "Speed" };
            _sortDropdown.index = 0;
        }

        private void SetupListView()
        {
            _characterListView.makeItem = MakeListItem;
            _characterListView.bindItem = BindListItem;
            _characterListView.selectionChanged += OnCharacterSelectionChanged;
            _characterListView.fixedItemHeight = 50;
        }

        private VisualElement MakeListItem()
        {
            var element = new VisualElement();
            element.AddToClassList("character-list-item");
    
            var characterPortraitImage = new Image();
            characterPortraitImage.name = "CharacterPortraitImage";
            characterPortraitImage.AddToClassList("character-portrait");
            element.Add(characterPortraitImage);
    
            var characterNameLabel = new Label();
            characterNameLabel.name = "CharacterNameLabel";
            characterNameLabel.AddToClassList("character-name");
            element.Add(characterNameLabel);
    
            return element;
        }

        private void BindListItem(VisualElement element, int index)
        {
            if (index < 0 || index >= _filteredCharacterList.Count)
                return;
            
            var character = _filteredCharacterList[index];
            
            var portraitImage = element.Q<Image>("CharacterPortraitImage");
            portraitImage.sprite = character.CharacterModel.Portrait;
            
            var nameLabel = element.Q<Label>("CharacterNameLabel");
            nameLabel.text = character.CharacterModel.Character.ToString();
        }

        private void OnCharacterSelectionChanged(IEnumerable<object> selectedItems)
        {
            if (!selectedItems.Any())
            {
                _selectedCharacter = null;
                _deleteButton.SetEnabled(false);
                ShowNoSelectionMessage();
                return;
            }
            
            _selectedCharacter = selectedItems.First() as CharacterScriptableObject;
            _deleteButton.SetEnabled(!_protectedCharacters.Contains(_selectedCharacter.CharacterModel.Character));
            ShowCharacterDetails(_selectedCharacter);
        }

        private void ShowNoSelectionMessage()
        {
            _noSelectionLabel.style.display = DisplayStyle.Flex;
            _characterDetailsView.style.display = DisplayStyle.None;
        }

        private void ShowCharacterDetails(CharacterScriptableObject character)
        {
            _noSelectionLabel.style.display = DisplayStyle.None;
            _characterDetailsView.style.display = DisplayStyle.Flex;
            
            _characterDetailsView.Clear();
            
            var editor = UnityEditor.Editor.CreateEditor(character);
            var imguiContainer = new IMGUIContainer(() => 
            {
                if (editor != null)
                {
                    editor.OnInspectorGUI();
                    if (GUI.changed)
                    {
                        EditorUtility.SetDirty(character);
                        RefreshCharacterList();
                        SortCharacterList();
                    }
                }
            });
            
            _characterDetailsView.Add(imguiContainer);
        }

        private void LoadDataSource()
        {
            _charactersScriptableObject = GetCharactersScriptableObject();
            if (_charactersScriptableObject == null)
            {
                Debug.LogError("CharactersScriptableObject not found. Please create one.");
                ShowCreateAssetDialog();
            }
        }

        private CharactersScriptableObject GetCharactersScriptableObject()
        {
            var guids = AssetDatabase.FindAssets("t:CharactersScriptableObject");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<CharactersScriptableObject>(path);
            }
            return null;
        }

        private void ShowCreateAssetDialog()
        {
            if (EditorUtility.DisplayDialog("Asset Missing", 
                "CharactersScriptableObject not found. Would you like to create one?", 
                "Yes", "No"))
            {
                CreateCharactersScriptableObject();
            }
        }

        private void CreateCharactersScriptableObject()
        {
            var charactersScriptableObject = CreateInstance<CharactersScriptableObject>();
            
            var path = EditorUtility.SaveFilePanelInProject(
                "Save CharactersScriptableObject",
                "Characters",
                "asset",
                "Please enter a filename to save the CharactersScriptableObject.");
            
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(charactersScriptableObject, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                _charactersScriptableObject = charactersScriptableObject;
            }
        }

        private void RefreshCharacterList()
        {
            if (_charactersScriptableObject == null)
                return;
            
            switch (_currentCharacterGroup)
            {
                case CharacterGroup.Hero:
                    _currentCharacterList = _charactersScriptableObject.Heroes.ToList();
                    break;
                case CharacterGroup.Enemy:
                    _currentCharacterList = _charactersScriptableObject.Enemies.ToList();
                    break;
                case CharacterGroup.Boss:
                    _currentCharacterList = _charactersScriptableObject.Heroes
                        .Concat(_charactersScriptableObject.Enemies)
                        .Where(c => (int)c.CharacterModel.Character >= (int)CharacterGroup.Boss && 
                               (int)c.CharacterModel.Character < (int)CharacterGroup.Boss + 10000)
                        .ToList();
                    break;
            }
            
            FilterCharacterList(_searchField.value);
        }

        private void FilterCharacterList(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                _filteredCharacterList = new List<CharacterScriptableObject>(_currentCharacterList);
            }
            else
            {
                searchString = searchString.ToLower();
                _filteredCharacterList = _currentCharacterList
                    .Where(c => c.CharacterModel.Character.ToString().ToLower().Contains(searchString))
                    .ToList();
            }
            
            SortCharacterList();
            UpdateListView();
        }

        private void SortCharacterList()
        {
            if (_filteredCharacterList == null)
                return;
            
            switch (_sortDropdown.index)
            {
                case 0: // Name
                    _filteredCharacterList = _filteredCharacterList
                        .OrderBy(c => c.CharacterModel.Character)
                        .ToList();
                    break;
                case 1: // Health
                    _filteredCharacterList = _filteredCharacterList
                        .OrderByDescending(c => c.CharacterModel.Health)
                        .ToList();
                    break;
                case 2: // Speed
                    _filteredCharacterList = _filteredCharacterList
                        .OrderByDescending(c => c.CharacterModel.Speed)
                        .ToList();
                    break;
            }
            
            UpdateListView();
        }

        private void UpdateListView()
        {
            _characterListView.itemsSource = _filteredCharacterList;
            _characterListView.Rebuild();
        }

        private void OnAddButtonClicked()
        {
            if (_charactersScriptableObject == null)
                return;
            
            CharacterCreationPopup.Show(this);
        }

        public void AddCharacterToList(CharacterScriptableObject newCharacter)
        {
            if (_charactersScriptableObject == null || newCharacter == null)
                return;
                
            switch (newCharacter.CharacterModel.CharacterGroup)
            {
                case CharacterGroup.Hero:
                    var heroes = _charactersScriptableObject.Heroes.ToList();
                    heroes.Add(newCharacter);
                    _charactersScriptableObject.Heroes = heroes.ToArray();
                    if (_currentCharacterGroup == CharacterGroup.Hero)
                    {
                        _currentCharacterList = heroes;
                    }
                    break;
                    
                case CharacterGroup.Enemy:
                    var enemies = _charactersScriptableObject.Enemies.ToList();
                    enemies.Add(newCharacter);
                    _charactersScriptableObject.Enemies = enemies.ToArray();
                    if (_currentCharacterGroup == CharacterGroup.Enemy)
                    {
                        _currentCharacterList = enemies;
                    }
                    break;
                    
                case CharacterGroup.Boss:
                    var bosses = _charactersScriptableObject.Enemies.ToList();
                    bosses.Add(newCharacter);
                    _charactersScriptableObject.Enemies = bosses.ToArray();
                    break;
            }
            
            EditorUtility.SetDirty(_charactersScriptableObject);
            AssetDatabase.SaveAssets();
            
            RefreshCharacterList();
            
            var newIndex = _filteredCharacterList.FindIndex(c => c.CharacterModel.Character == newCharacter.CharacterModel.Character);
            if (newIndex >= 0)
            {
                _characterListView.SetSelection(newIndex);
            }
        }

        private void OnDeleteButtonClicked()
        {
            if (_selectedCharacter == null || _charactersScriptableObject == null)
                return;
            
            if (_protectedCharacters.Contains(_selectedCharacter.CharacterModel.Character))
            {
                EditorUtility.DisplayDialog("Protected Character", 
                    $"Cannot delete {_selectedCharacter.CharacterModel.Character} as it is a protected character.", 
                    "OK");
                return;
            }
            
            if (EditorUtility.DisplayDialog("Delete Character", 
                $"Are you sure you want to delete {_selectedCharacter.CharacterModel.Character}?", 
                "Yes", "No"))
            {
                RemoveCharacterFromEnum(_selectedCharacter.CharacterModel.Character);
                
                switch (_selectedCharacter.CharacterModel.CharacterGroup)
                {
                    case CharacterGroup.Hero:
                        var heroes = _charactersScriptableObject.Heroes.ToList();
                        heroes.Remove(_selectedCharacter);
                        _charactersScriptableObject.Heroes = heroes.ToArray();
                        break;
                        
                    case CharacterGroup.Enemy:
                    case CharacterGroup.Boss:
                        var enemies = _charactersScriptableObject.Enemies.ToList();
                        enemies.Remove(_selectedCharacter);
                        _charactersScriptableObject.Enemies = enemies.ToArray();
                        break;
                }
                
                var assetPath = AssetDatabase.GetAssetPath(_selectedCharacter);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
                
                EditorUtility.SetDirty(_charactersScriptableObject);
                AssetDatabase.SaveAssets();
                
                RefreshCharacterList();
                ShowNoSelectionMessage();
            }
        }
        
        private void RemoveCharacterFromEnum(Character characterToRemove)
        {
            var characterName = characterToRemove.ToString();
            
            var lines = File.ReadAllLines(EnumFilePath);
            var newLines = new List<string>();
            
            int i = 0;
            while (i < lines.Length)
            {
                var line = lines[i];
                var trimmedLine = line.Trim();
                
                if (trimmedLine.StartsWith(characterName + " = ") || trimmedLine.StartsWith(characterName + "="))
                {
                    i++;
                    continue;
                }
                
                newLines.Add(line);
                i++;
            }
            
            File.WriteAllLines(EnumFilePath, newLines);
            AssetDatabase.ImportAsset(EnumFilePath);
        }
    }
}