using System.Collections.Generic;
using System.Linq;
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
            _mainContainer = rootVisualElement.Q<VisualElement>("MainContainer") ?? _mainContainer;
            _toolbarContainer = rootVisualElement.Q<VisualElement>("ToolbarContainer") ?? _toolbarContainer;
            _listContainer = rootVisualElement.Q<VisualElement>("ListContainer") ?? _listContainer;
            _detailsContainer = rootVisualElement.Q<VisualElement>("DetailsContainer") ?? _detailsContainer;
            
            _heroesToggle = rootVisualElement.Q<ToolbarToggle>("HeroesToggle") ?? _heroesToggle;
            _enemiesToggle = rootVisualElement.Q<ToolbarToggle>("EnemiesToggle") ?? _enemiesToggle;
            
            _searchField = rootVisualElement.Q<TextField>("SearchField") ?? _searchField;
            _sortDropdown = rootVisualElement.Q<DropdownField>("SortDropdown") ?? _sortDropdown;
            
            _characterListView = rootVisualElement.Q<ListView>("CharacterListView") ?? _characterListView;
            _noSelectionLabel = rootVisualElement.Q<Label>("NoSelectionLabel") ?? _noSelectionLabel;
            _characterDetailsView = rootVisualElement.Q<ScrollView>("CharacterDetailsView") ?? _characterDetailsView;
            
            _addButton = rootVisualElement.Q<Button>("AddButton") ?? _addButton;
            _deleteButton = rootVisualElement.Q<Button>("DeleteButton") ?? _deleteButton;
            
            SetupListView();
        }

        private void RegisterCallbacks()
        {
            _heroesToggle.RegisterValueChangedCallback(evt => 
            {
                if (evt.newValue)
                {
                    _enemiesToggle.SetValueWithoutNotify(false);
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
                    _currentCharacterGroup = CharacterGroup.Enemy;
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
            
            // var element = new VisualElement();
            // element.style.flexDirection = FlexDirection.Row;
            // element.style.flexGrow = 1;
            // element.style.height = 50;
            // element.style.borderBottomWidth = 1;
            // element.style.borderBottomColor = new Color(0.1f, 0.1f, 0.1f);
            // element.style.paddingLeft = 5;
            // element.style.paddingRight = 5;
            //
            // var characterPortraitImage = new Image();
            // characterPortraitImage.name = "CharacterPortraitImage";
            // characterPortraitImage.style.flexGrow = 1;
            // characterPortraitImage.style.width = 50;
            // characterPortraitImage.style.height = 50;
            // element.Add(characterPortraitImage);
            //
            // // var characterIndexLabel = new Label();
            // // characterIndexLabel.name = "CharacterIndexLabel";
            // // characterIndexLabel.style.width = 30;
            // // characterIndexLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            // // element.Add(characterIndexLabel);
            //
            // var characterNameLabel = new Label();
            // characterNameLabel.name = "CharacterNameLabel";
            // characterNameLabel.style.paddingLeft = 10;
            // characterNameLabel.style.flexGrow = 1;
            // characterNameLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            // element.Add(characterNameLabel);
            //
            // return element;
        }

        private void BindListItem(VisualElement element, int index)
        {
            if (index < 0 || index >= _filteredCharacterList.Count)
                return;
            
            var character = _filteredCharacterList[index];
            
            var portraitImage = element.Q<Image>("CharacterPortraitImage");
            portraitImage.sprite = character.CharacterModel.Portrait;
            
            // var indexLabel = element.Q<Label>("CharacterIndexLabel");
            // indexLabel.text = ((int)character.CharacterModel.Character).ToString();
            
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
            _deleteButton.SetEnabled(true);
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
            
            _currentCharacterList = _currentCharacterGroup == CharacterGroup.Hero 
                ? _charactersScriptableObject.Heroes.ToList() 
                : _charactersScriptableObject.Enemies.ToList();
            
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
                case 0: // Character
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
            
            var newCharacter = CreateCharacterScriptableObject();
            if (newCharacter != null)
            {
                if (_currentCharacterGroup == CharacterGroup.Hero)
                {
                    var heroes = _charactersScriptableObject.Heroes.ToList();
                    heroes.Add(newCharacter);
                    _charactersScriptableObject.Heroes = heroes.ToArray();
                }
                else
                {
                    var enemies = _charactersScriptableObject.Enemies.ToList();
                    enemies.Add(newCharacter);
                    _charactersScriptableObject.Enemies = enemies.ToArray();
                }
                
                EditorUtility.SetDirty(_charactersScriptableObject);
                AssetDatabase.SaveAssets();
                
                RefreshCharacterList();
                _characterListView.SetSelection(_filteredCharacterList.Count - 1);
            }
        }

        private void OnDeleteButtonClicked()
        {
            EditorUtility.SetDirty(_charactersScriptableObject);
            AssetDatabase.SaveAssets();
            
            RefreshCharacterList();
            ShowNoSelectionMessage();
        }

        private CharacterScriptableObject CreateCharacterScriptableObject()
        {
            var newCharacter = CreateInstance<CharacterScriptableObject>();
            newCharacter.CharacterModel = new CharacterModel
            {
                CharacterGroup = _currentCharacterGroup,
                // Index = GetNextCharacterIndex(),
                Health = 100,
                Speed = 5,
                Skills = new List<SkillModel>()
            };
            
            var path = EditorUtility.SaveFilePanelInProject(
                "Save Character",
                newCharacter.GetAssetName(),
                "asset",
                "Please enter a filename to save the Character.");
            
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newCharacter, path);
                AssetDatabase.SaveAssets();
                return newCharacter;
            }
            
            return null;
        }

        private int GetNextCharacterIndex()
        {
            var currentCharacterGroupIndex = (int)_currentCharacterGroup;
            var nextCharacterIndex = currentCharacterGroupIndex;

            foreach (var characterScriptableObject in _currentCharacterList)
            {
                var currentCharacterIndex = (int)characterScriptableObject.CharacterModel.Character;
                if (currentCharacterIndex < currentCharacterGroupIndex)
                {
                    continue;
                }

                if (currentCharacterIndex >= nextCharacterIndex)
                {
                    if (currentCharacterIndex - nextCharacterIndex > 1)
                    {
                        break;
                    }

                    ++nextCharacterIndex;
                }
            }

            return nextCharacterIndex;
        }
    }
}