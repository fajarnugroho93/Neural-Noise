using SpaceKomodo.TurnBasedSystem.Maps;
using SpaceKomodo.Utilities;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class MapGridView : MonoBehaviour, IInitializable<MapGridModel>
    {
        [SerializeField] private SpriteRenderer _gridRenderer;
        
        private MapGridModel _model;
        
        public void Initialize(MapGridModel model)
        {
            _model = model;
            name = $"MapGridView_{model.MapGrid}_{model.Column}_{model.Row}";
        }
        
        public MapGridModel Model => _model;
    }
}