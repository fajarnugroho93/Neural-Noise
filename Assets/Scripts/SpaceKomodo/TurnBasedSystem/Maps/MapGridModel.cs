namespace SpaceKomodo.TurnBasedSystem.Maps
{
    public class MapGridModel
    {
        public readonly MapGrid MapGrid;
        public readonly int Column;
        public readonly int Row;
        public MapCharacterModel MapCharacterModel;

        public MapGridModel(
            MapGrid mapGrid, 
            int column, 
            int row)
        {
            MapGrid = mapGrid;
            Column = column;
            Row = row;
        }

        public void SetMapCharacterModel(MapCharacterModel mapCharacterModel)
        {
            MapCharacterModel = mapCharacterModel;
        }
    }
}