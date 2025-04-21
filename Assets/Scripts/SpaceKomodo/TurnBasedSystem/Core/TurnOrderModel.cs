using System.Linq;
using ObservableCollections;
using SpaceKomodo.TurnBasedSystem.Characters;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Core
{
    public class TurnOrderModel
    {
        private readonly ObservableList<CharacterModel> _models;
        
        public TurnOrderModel(ObservableList<CharacterModel> models)
        {
            _models = models;
        }
        
        public void ShiftTurnOrder(CharacterModel model, int shift)
        {
            var modelsArray = _models.ToArray();
            var currentIndex = -1;
            
            for (int i = 0; i < modelsArray.Length; i++)
            {
                if (modelsArray[i].TurnOrder.Value == model.TurnOrder.Value)
                {
                    currentIndex = i;
                    break;
                }
            }
            
            if (currentIndex == -1) return;
            
            var targetIndex = Mathf.Clamp(currentIndex + shift, 0, modelsArray.Length - 1);
            if (targetIndex == currentIndex) return;
            
            var targetTurnOrder = modelsArray[targetIndex].TurnOrder.Value;
            
            if (targetIndex < currentIndex)
            {
                for (int i = 0; i < modelsArray.Length; i++)
                {
                    var turnOrder = modelsArray[i].TurnOrder.Value;
                    if (turnOrder >= targetTurnOrder && turnOrder < model.TurnOrder.Value)
                    {
                        modelsArray[i].TurnOrder.Value = turnOrder + 1;
                    }
                }
            }
            else
            {
                for (int i = 0; i < modelsArray.Length; i++)
                {
                    var turnOrder = modelsArray[i].TurnOrder.Value;
                    if (turnOrder <= targetTurnOrder && turnOrder > model.TurnOrder.Value)
                    {
                        modelsArray[i].TurnOrder.Value = turnOrder - 1;
                    }
                }
            }
            
            model.TurnOrder.Value = targetTurnOrder;
        }
    }
}