using MessagePipe;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Events;
using SpaceKomodo.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class CurrentTurnSkillView : MonoBehaviour, IInitializable<SkillModel>
    {
        public Image Portrait;
        public Button Button;
        public GameObject SelectableGameObject;
        public GameObject SelectedGameObject;
        public DiceFaceView DiceFaceView;
        
        private SkillModel _skillModel;
        private IPublisher<SkillClickedEvent> _skillClickedPublisher;
        
        [Inject]
        public void Construct(IPublisher<SkillClickedEvent> skillClickedPublisher)
        {
            _skillClickedPublisher = skillClickedPublisher;
        }
        
        public void Initialize(SkillModel skillModel)
        {
            _skillModel = skillModel;
            Portrait.sprite = skillModel.Portrait;
            
            Button.onClick.AddListener(OnSkillClicked);
        }
        
        private void OnSkillClicked()
        {
            _skillClickedPublisher.Publish(new SkillClickedEvent(_skillModel));
        }
    }
}