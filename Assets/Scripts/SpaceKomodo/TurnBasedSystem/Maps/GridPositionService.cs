using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Maps
{
    public class GridPositionService : IGridPositionService
    {
        private readonly Vector3 _gridSize = new(1.1f, 0, -1.1f);
        private readonly Vector3 _gridOffset = new(0.55f, 0, 0);
        
        public Vector3 GetLocalPosition(MapGridModel model)
        {
            var basePosition = new Vector3(
                model.Column * _gridSize.x + model.Row * _gridOffset.x,
                0,
                model.Row * _gridSize.z
            );
            
            if (model.MapGrid == MapGrid.HeroGrid)
            {
                basePosition.x = -basePosition.x;
            }
            
            return new Vector3(basePosition.x, 0, basePosition.z);
        }
        
        public Vector2 GetCellSize()
        {
            return _gridSize;
        }
    }
}