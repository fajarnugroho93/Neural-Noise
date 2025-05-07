using MessagePipe;
using R3;
using SpaceKomodo.TurnBasedSystem.Dice;
using SpaceKomodo.TurnBasedSystem.Events;
using SpaceKomodo.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using DisposableBag = R3.DisposableBag;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class TurnDiceView : MonoBehaviour, IInitializable<DiceModel>
    {
        public Button Button;
        public DiceFaceView DiceFaceView;
        public GameObject SelectableGameObject;
        public GameObject SelectedGameObject;
        
        private DiceModel _diceModel;
        private IPublisher<DiceClickedEvent> _turnDiceClickedPublisher;
        private DisposableBag _disposableBag;
        
        [Inject]
        public void Construct(IPublisher<DiceClickedEvent> turnDiceClickedPublisher)
        {
            _turnDiceClickedPublisher = turnDiceClickedPublisher;
        }
        
        public void Initialize(DiceModel diceModel)
        {
            _diceModel = diceModel;
            _diceModel.Value.Subscribe(OnDiceModelValueChanged).AddTo(ref _disposableBag);

            Button.onClick.AddListener(OnDiceTurnClicked);
            
            _diceModel.IsSelectable
                .Subscribe(OnSelectableChanged)
                .AddTo(ref _disposableBag);
            _diceModel.IsSelected
                .Subscribe(OnSelectedChanged)
                .AddTo(ref _disposableBag);

            void OnSelectableChanged(bool isSelectable)
            {
                if (SelectableGameObject != null)
                {
                    SelectableGameObject.SetActive(isSelectable);
                }
            }

            void OnSelectedChanged(bool isSelected)
            {
                if (SelectedGameObject != null)
                {
                    SelectedGameObject.SetActive(isSelected);
                }
            }

            void OnDiceModelValueChanged(int value)
            {
                DiceFaceView.DiceText.text = value.ToString();
            }
        }
        
        private void OnDiceTurnClicked()
        {
            _turnDiceClickedPublisher.Publish(new DiceClickedEvent(_diceModel));
        }

        private void OnDestroy()
        {
            _disposableBag.Dispose();
        }
    }
}