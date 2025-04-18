using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Maps
{
    public interface IGridPositionService
    {
        Vector3 GetLocalPosition(MapGridModel model);
        Vector2 GetCellSize();
    }
}