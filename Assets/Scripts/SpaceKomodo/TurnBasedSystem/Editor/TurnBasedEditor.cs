using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceKomodo.TurnBasedSystem.Editor
{
    public class TurnBasedEditor : EditorWindow
    {
        private const string WindowTitle = "Turn Based Editor";
        private const string CharacterEnumFilePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Character.cs";
        private const string SkillEnumFilePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/Skill.cs";

        private const string SkillEffectEnumFilePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/SkillEffect.cs";

        private enum EditorTab
        {
            Characters,
            Skills,
            Effects
        }

        private EditorTab _currentTab = EditorTab.Characters;

        private CharactersScriptableObject _charactersScriptableObject;
        private List<CharacterScriptableObject> _currentCharacterList = new();
        private List<CharacterScriptableObject> _filteredCharacterList = new();

        private Skill[] _skillList = new Skill[0];
        private List<Skill> _filteredSkillList = new();

        private VisualElement _mainContainer;
        private VisualElement _tabsContainer;
        private VisualElement _toolbarContainer;
        private VisualElement _listContainer;
        private VisualElement _detailsContainer;

        private Button _charactersTabButton;
        private Button _skillsTabButton;
        private Button _effectsTabButton;

        private TextField _searchField;
        private DropdownField _sortDropdown;
        private ListView _listView;
        private ScrollView _detailsView;
        private Label _noSelectionLabel;

        private Button _addButton;
        private Button _deleteButton;

        private ToolbarToggle _heroesToggle;
        private ToolbarToggle _enemiesToggle;
        private ToolbarToggle _bossesToggle;

        private CharacterGroup _currentCharacterGroup = CharacterGroup.Hero;

        private object _selectedItem;

        private readonly List<Character> _protectedCharacters = new()
        {
            Character.None,
            Character.Warrior,
            Character.Mossball,
            Character.Mossking
        };

        private readonly List<Skill> _protectedSkills = new()
        {
            Skill.None,
        };

        [MenuItem("Tools/TurnBasedSystem/Turn Based Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<TurnBasedEditor>();
            window.titleContent = new GUIContent(WindowTitle);
            window.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            LoadUIElements();
            LoadDataSources();
            SetupSortDropdown();
            UpdateListView();
        }

        private void LoadUIElements()
        {
            var root = rootVisualElement;

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/SpaceKomodo/TurnBasedSystem/Editor/TurnBasedEditor.uss");
            if (styleSheet != null)
            {
                root.styleSheets.Add(styleSheet);
            }

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/SpaceKomodo/TurnBasedSystem/Editor/TurnBasedEditor.uxml");
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
            _tabsContainer = rootVisualElement.Q<VisualElement>("TabsContainer");
            _toolbarContainer = rootVisualElement.Q<VisualElement>("ToolbarContainer");
            _listContainer = rootVisualElement.Q<VisualElement>("ListContainer");
            _detailsContainer = rootVisualElement.Q<VisualElement>("DetailsContainer");

            _charactersTabButton = rootVisualElement.Q<Button>("CharactersTab");
            _skillsTabButton = rootVisualElement.Q<Button>("SkillsTab");
            _effectsTabButton = rootVisualElement.Q<Button>("EffectsTab");

            _heroesToggle = rootVisualElement.Q<ToolbarToggle>("HeroesToggle");
            _enemiesToggle = rootVisualElement.Q<ToolbarToggle>("EnemiesToggle");
            _bossesToggle = rootVisualElement.Q<ToolbarToggle>("BossesToggle");

            _searchField = rootVisualElement.Q<TextField>("SearchField");
            _sortDropdown = rootVisualElement.Q<DropdownField>("SortDropdown");

            _listView = rootVisualElement.Q<ListView>("ListView");
            _noSelectionLabel = rootVisualElement.Q<Label>("NoSelectionLabel");
            _detailsView = rootVisualElement.Q<ScrollView>("DetailsView");

            _addButton = rootVisualElement.Q<Button>("AddButton");
            _deleteButton = rootVisualElement.Q<Button>("DeleteButton");

            SetupListView();
        }

        private void RegisterCallbacks()
        {
            _charactersTabButton.clicked += () =>
            {
                _currentTab = EditorTab.Characters;
                UpdateTabSelection();
                UpdateGroupTogglesVisibility();
                UpdateListView();
            };

            _skillsTabButton.clicked += () =>
            {
                _currentTab = EditorTab.Skills;
                UpdateTabSelection();
                UpdateGroupTogglesVisibility();
                UpdateListView();
            };

            _effectsTabButton.clicked += () =>
            {
                _currentTab = EditorTab.Effects;
                UpdateTabSelection();
                UpdateGroupTogglesVisibility();
                UpdateListView();
            };

            _heroesToggle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    _enemiesToggle.SetValueWithoutNotify(false);
                    _bossesToggle.SetValueWithoutNotify(false);
                    _currentCharacterGroup = CharacterGroup.Hero;
                    RefreshCharacterList();
                    _selectedItem = null;
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
                    _selectedItem = null;
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
                    _selectedItem = null;
                    ShowNoSelectionMessage();
                }
            });

            _searchField.RegisterValueChangedCallback(evt => FilterList(evt.newValue));
            _sortDropdown.RegisterValueChangedCallback(evt => SortList());

            _addButton.clicked += OnAddButtonClicked;
            _deleteButton.clicked += OnDeleteButtonClicked;
        }

        private void UpdateTabSelection()
        {
            _charactersTabButton.RemoveFromClassList("tab-selected");
            _skillsTabButton.RemoveFromClassList("tab-selected");
            _effectsTabButton.RemoveFromClassList("tab-selected");

            switch (_currentTab)
            {
                case EditorTab.Characters:
                    _charactersTabButton.AddToClassList("tab-selected");
                    break;
                case EditorTab.Skills:
                    _skillsTabButton.AddToClassList("tab-selected");
                    break;
                case EditorTab.Effects:
                    _effectsTabButton.AddToClassList("tab-selected");
                    break;
            }
            
            _sortDropdown.choices = GetSortOptionsForCurrentTab();
            _sortDropdown.index = 0;

            _selectedItem = null;
            ShowNoSelectionMessage();
        }

        private void UpdateGroupTogglesVisibility()
        {
            var showGroupToggles = _currentTab == EditorTab.Characters;
            _heroesToggle.style.display = showGroupToggles ? DisplayStyle.Flex : DisplayStyle.None;
            _enemiesToggle.style.display = showGroupToggles ? DisplayStyle.Flex : DisplayStyle.None;
            _bossesToggle.style.display = showGroupToggles ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void SetupSortDropdown()
        {
            _sortDropdown.choices = GetSortOptionsForCurrentTab();
            _sortDropdown.index = 0;
        }

        private List<string> GetSortOptionsForCurrentTab()
        {
            switch (_currentTab)
            {
                case EditorTab.Characters:
                    return new List<string> { "Value", "Name", "Health", "Speed" };
                case EditorTab.Skills:
                    return new List<string> { "Value", "Name" };
                case EditorTab.Effects:
                    return new List<string> { "Value", "Name" };
                default:
                    return new List<string>();
            }
        }

        private void SetupListView()
        {
            _listView.makeItem = MakeListItem;
            _listView.bindItem = BindListItem;
            _listView.selectionChanged += OnSelectionChanged;
            _listView.fixedItemHeight = 50;
        }

        private VisualElement MakeListItem()
        {
            var element = new VisualElement();
            element.AddToClassList("list-item");

            var iconImage = new Image();
            iconImage.name = "ItemIcon";
            iconImage.AddToClassList("item-icon");
            element.Add(iconImage);

            var nameLabel = new Label();
            nameLabel.name = "ItemNameLabel";
            nameLabel.AddToClassList("item-name");
            element.Add(nameLabel);

            return element;
        }

        private void BindListItem(VisualElement element, int index)
        {
            if (index < 0)
                return;

            var iconImage = element.Q<Image>("ItemIcon");
            var nameLabel = element.Q<Label>("ItemNameLabel");

            switch (_currentTab)
            {
                case EditorTab.Characters:
                    if (index < _filteredCharacterList.Count)
                    {
                        var character = _filteredCharacterList[index];
                        iconImage.sprite = character.CharacterModel.Portrait;
                        nameLabel.text = character.CharacterModel.Character.ToString();
                    }

                    break;
                case EditorTab.Skills:
                    if (index < _filteredSkillList.Count)
                    {
                        var skill = _filteredSkillList[index];
                        iconImage.sprite = null;
                        nameLabel.text = skill.ToString();
                    }

                    break;
            }
        }

        private void OnSelectionChanged(IEnumerable<object> selectedItems)
        {
            if (!selectedItems.Any())
            {
                _selectedItem = null;
                _deleteButton.SetEnabled(false);
                ShowNoSelectionMessage();
                return;
            }

            _selectedItem = selectedItems.First();

            switch (_currentTab)
            {
                case EditorTab.Characters:
                    var character = _selectedItem as CharacterScriptableObject;
                    _deleteButton.SetEnabled(character != null &&
                                             !_protectedCharacters.Contains(character.CharacterModel.Character));
                    if (character != null)
                    {
                        ShowCharacterDetails(character);
                    }
                    break;
                
                case EditorTab.Skills:
                    var skill = (Skill)_selectedItem;
                    _deleteButton.SetEnabled(!_protectedSkills.Contains(skill));
                    ShowSkillDetails(skill);
                    break;
            }
        }

        private void ShowNoSelectionMessage()
        {
            _noSelectionLabel.style.display = DisplayStyle.Flex;
            _detailsView.style.display = DisplayStyle.None;

            switch (_currentTab)
            {
                case EditorTab.Characters:
                    _noSelectionLabel.text = "No character selected";
                    break;
                case EditorTab.Skills:
                    _noSelectionLabel.text = "No skill selected";
                    break;
                case EditorTab.Effects:
                    _noSelectionLabel.text = "No effect selected";
                    break;
            }
        }

        private void ShowCharacterDetails(CharacterScriptableObject character)
        {
            _noSelectionLabel.style.display = DisplayStyle.None;
            _detailsView.style.display = DisplayStyle.Flex;

            _detailsView.Clear();

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
                        SortList();
                    }
                }
            });

            _detailsView.Add(imguiContainer);
        }

        private void ShowSkillDetails(Skill skill)
        {
            _noSelectionLabel.style.display = DisplayStyle.None;
            _detailsView.style.display = DisplayStyle.Flex;

            _detailsView.Clear();

            var container = new VisualElement();
            container.style.paddingTop = 10;

            var nameField = new TextField("Skill Name");
            nameField.SetEnabled(false);
            nameField.value = skill.ToString();
            container.Add(nameField);

            var valueField = new IntegerField("Value");
            valueField.SetEnabled(false);
            valueField.value = (int)skill;
            container.Add(valueField);

            _detailsView.Add(container);
        }

        private void LoadDataSources()
        {
            LoadCharactersData();
            LoadSkillsData();
        }

        private void LoadCharactersData()
        {
            _charactersScriptableObject = GetCharactersScriptableObject();
            if (_charactersScriptableObject == null)
            {
                Debug.LogError("CharactersScriptableObject not found. Please create one.");
                return;
            }

            RefreshCharacterList();
        }

        private void LoadSkillsData()
        {
            _skillList = (Skill[])Enum.GetValues(typeof(Skill));
            RefreshSkillList();
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

            FilterList(_searchField.value);
        }

        private void RefreshSkillList()
        {
            FilterList(_searchField.value);
        }

        private void RefreshEffectsList()
        {
            FilterList(_searchField.value);
        }

        private void FilterList(string searchString)
        {
            switch (_currentTab)
            {
                case EditorTab.Characters:
                    FilterCharacterList(searchString);
                    break;
                case EditorTab.Skills:
                    FilterSkillList(searchString);
                    break;
            }
            
            _sortDropdown.choices = GetSortOptionsForCurrentTab();
            if (_sortDropdown.index >= _sortDropdown.choices.Count)
            {
                _sortDropdown.index = 0;
            }

            SortList();
            UpdateListView();
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
        }

        private void FilterSkillList(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                _filteredSkillList = new List<Skill>(_skillList);
            }
            else
            {
                searchString = searchString.ToLower();
                _filteredSkillList = _skillList
                    .Where(s => s.ToString().ToLower().Contains(searchString))
                    .ToList();
            }
        }

        private void SortList()
        {
            if (_sortDropdown == null || _sortDropdown.index < 0)
                return;

            switch (_currentTab)
            {
                case EditorTab.Characters:
                    SortCharacterList();
                    break;
                case EditorTab.Skills:
                    SortSkillList();
                    break;
            }

            UpdateListView();
        }

        private void SortCharacterList()
        {
            if (_filteredCharacterList == null)
                return;

            switch (_sortDropdown.index)
            {
                case 0: // Value
                    _filteredCharacterList = _filteredCharacterList
                        .OrderBy(c => (int)c.CharacterModel.Character)
                        .ToList();
                    break;
                case 1: // Name
                    _filteredCharacterList = _filteredCharacterList
                        .OrderBy(c => c.CharacterModel.Character.ToString())
                        .ToList();
                    break;
                case 2: // Health
                    _filteredCharacterList = _filteredCharacterList
                        .OrderByDescending(c => c.CharacterModel.Health)
                        .ToList();
                    break;
                case 3: // Speed
                    _filteredCharacterList = _filteredCharacterList
                        .OrderByDescending(c => c.CharacterModel.Speed)
                        .ToList();
                    break;
            }
        }

        private void SortSkillList()
        {
            if (_filteredSkillList == null)
                return;

            switch (_sortDropdown.index)
            {
                case 0: // Value
                    _filteredSkillList = _filteredSkillList
                        .OrderBy(s => (int)s)
                        .ToList();
                    break;
                case 1: // Name
                    _filteredSkillList = _filteredSkillList
                        .OrderBy(s => s.ToString())
                        .ToList();
                    break;
            }
        }

        private void UpdateListView()
        {
            switch (_currentTab)
            {
                case EditorTab.Characters:
                    _listView.itemsSource = _filteredCharacterList;
                    break;
                case EditorTab.Skills:
                    _listView.itemsSource = _filteredSkillList;
                    break;
            }

            _listView.Rebuild();
        }

        private void OnAddButtonClicked()
        {
            switch (_currentTab)
            {
                case EditorTab.Characters:
                    OnAddCharacterClicked();
                    break;
                case EditorTab.Skills:
                    OnAddSkillClicked();
                    break;
                case EditorTab.Effects:
                    OnAddEffectClicked();
                    break;
            }
        }

        private void OnAddCharacterClicked()
        {
            if (_charactersScriptableObject == null)
                return;

            CharacterCreationPopup.Show(this);
        }

        private void OnAddSkillClicked()
        {
            SkillCreationPopup.Show(this);
        }

        private void OnAddEffectClicked()
        {
            EffectCreationPopup.Show(this);
        }

        private void OnDeleteButtonClicked()
        {
            switch (_currentTab)
            {
                case EditorTab.Characters:
                    OnDeleteCharacterClicked();
                    break;
                case EditorTab.Skills:
                    OnDeleteSkillClicked();
                    break;
                case EditorTab.Effects:
                    OnDeleteEffectClicked();
                    break;
            }
        }

        private void OnDeleteCharacterClicked()
        {
            var character = _selectedItem as CharacterScriptableObject;
            if (character == null || _charactersScriptableObject == null)
                return;

            if (_protectedCharacters.Contains(character.CharacterModel.Character))
            {
                EditorUtility.DisplayDialog("Protected Character",
                    $"Cannot delete {character.CharacterModel.Character} as it is a protected character.",
                    "OK");
                return;
            }

            if (EditorUtility.DisplayDialog("Delete Character",
                    $"Are you sure you want to delete {character.CharacterModel.Character}?",
                    "Yes", "No"))
            {
                RemoveEnumEntry(CharacterEnumFilePath, character.CharacterModel.Character.ToString());

                switch (character.CharacterModel.CharacterGroup)
                {
                    case CharacterGroup.Hero:
                        var heroes = _charactersScriptableObject.Heroes.ToList();
                        heroes.Remove(character);
                        _charactersScriptableObject.Heroes = heroes.ToArray();
                        break;

                    case CharacterGroup.Enemy:
                    case CharacterGroup.Boss:
                        var enemies = _charactersScriptableObject.Enemies.ToList();
                        enemies.Remove(character);
                        _charactersScriptableObject.Enemies = enemies.ToArray();
                        break;
                }

                var assetPath = AssetDatabase.GetAssetPath(character);
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

        private void OnDeleteSkillClicked()
        {
            if (!(_selectedItem is Skill skill))
                return;

            if (_protectedSkills.Contains(skill))
            {
                EditorUtility.DisplayDialog("Protected Skill",
                    $"Cannot delete {skill} as it is a protected skill.",
                    "OK");
                return;
            }

            if (EditorUtility.DisplayDialog("Delete Skill",
                    $"Are you sure you want to delete {skill}? This may break references in skill data.",
                    "Yes", "No"))
            {
                RemoveEnumEntry(SkillEnumFilePath, skill.ToString());
                AssetDatabase.SaveAssets();

                LoadSkillsData();
                ShowNoSelectionMessage();
            }
        }

        private void OnDeleteEffectClicked()
        {
            
        }

        private void RemoveEnumEntry(string filePath, string entryName)
        {
            var lines = File.ReadAllLines(filePath);
            var newLines = new List<string>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith(entryName + " = ") || trimmedLine.StartsWith(entryName + "="))
                {
                    continue;
                }

                newLines.Add(line);
            }

            File.WriteAllLines(filePath, newLines);
            AssetDatabase.ImportAsset(filePath);
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

            var newIndex = _filteredCharacterList.FindIndex(c =>
                c.CharacterModel.Character == newCharacter.CharacterModel.Character);
            if (newIndex >= 0)
            {
                _listView.SetSelection(newIndex);
            }
        }

        public void AddSkillToEnum(Skill skill)
        {
            LoadSkillsData();

            var newIndex = _filteredSkillList.IndexOf(skill);
            if (newIndex >= 0)
            {
                _listView.SetSelection(newIndex);
            }
        }
    } 
}