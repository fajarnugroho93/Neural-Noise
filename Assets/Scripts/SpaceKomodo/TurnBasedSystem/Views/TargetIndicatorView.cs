using MessagePipe;
using SpaceKomodo.TurnBasedSystem.Events;
using SpaceKomodo.TurnBasedSystem.Maps;
using SpaceKomodo.Utilities;
using UnityEngine;
using VContainer;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class TargetIndicatorView : MonoBehaviour, IInitializable<MapCharacterModel>
    {
        [SerializeField] private GameObject _selectedGameObject;
        [SerializeField] private GameObject _notSelectedGameObject;
        
        private MapCharacterModel _model;
        private IPublisher<TargetClickedEvent> _targetClickedPublisher;
        
        [Inject]
        public void Construct(IPublisher<TargetClickedEvent> targetClickedPublisher)
        {
            _targetClickedPublisher = targetClickedPublisher;
        }
        
        public void Initialize(MapCharacterModel model)
        {
            _model = model;
            name = $"TargetIndicator_{model.CharacterModel.Character}";
            
            SetSelectedState(false);
        }
        
        private void OnMouseDown()
        {
            _targetClickedPublisher.Publish(new TargetClickedEvent(_model.CharacterModel));
        }
        
        public void SetSelectedState(bool isSelected)
        {
            _selectedGameObject.SetActive(isSelected);
            _notSelectedGameObject.SetActive(!isSelected);
        }
    }
}