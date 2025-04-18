using SpaceKomodo.TurnBasedSystem.Characters;

namespace SpaceKomodo.TurnBasedSystem.Maps
{
    public class MapModel
    {
        public MapGridModel[,] HeroMapGrids;
        public MapGridModel[,] EnemyMapGrids;

        public int HeroGridCounter;
        public int EnemyGridCounter;

        public MapModel(
            int column,
            int row)
        {
            HeroMapGrids = CreateMapGridsModel(MapGrid.HeroGrid, column, row);
            EnemyMapGrids = CreateMapGridsModel(MapGrid.EnemyGrid, column, row);
        }

        private MapGridModel[,] CreateMapGridsModel(
            MapGrid mapGrid, 
            int column, 
            int row)
        {
            var mapGridsModel = new MapGridModel[column, row];
            for (var ii = 0; ii < column; ++ii)
            {
                for (var jj = 0; jj < row; ++jj)
                {
                    mapGridsModel[ii, jj] = new MapGridModel(mapGrid, column, row);
                }
            }

            return mapGridsModel;
        }

        public MapCharacterModel AddModel(MapGrid mapGrid, CharacterModel newModel)
        {
            var mapGridsModel = HeroMapGrids;
            if (mapGrid == MapGrid.EnemyGrid)
            {
                mapGridsModel = EnemyMapGrids;
            }

            var mapGridIndex = 0; 
            if (mapGrid == MapGrid.HeroGrid)
            {
                mapGridIndex = HeroGridCounter;
                ++HeroGridCounter;
            }
            else
            {
                mapGridIndex = EnemyGridCounter;
                ++EnemyGridCounter;
            }

            var mapGridModel = mapGridsModel[0, mapGridIndex];
            var mapCharacterModel = new MapCharacterModel(newModel, mapGridModel); 
            mapGridModel.SetMapCharacterModel(mapCharacterModel);
            
            return mapCharacterModel;
        }
    }
}