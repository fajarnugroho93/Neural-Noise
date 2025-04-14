using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class CurrentTurnSkillView : MonoBehaviour, IInitializable<SkillModel>
    {
        public Image Portrait;
        
        public void Initialize(SkillModel skillModel)
        {
            Portrait.sprite = skillModel.Portrait;
        }
    }
}