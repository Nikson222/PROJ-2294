using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game
{
    public class LevelLoader
    {
        private readonly CubeFactory _factory;
        private readonly LevelSelectionService _selection;
        private readonly BoardView _boardView;
        private readonly CubeMover _cubeMover;
        private readonly WallTileConfig _wallTileConfig;

        public LevelLoader(
            CubeFactory factory,
            LevelSelectionService selection,
            BoardView boardView,
            CubeMover cubeMover,
            WallTileConfig wallTileConfig)
        {
            _factory = factory;
            _selection = selection;
            _boardView = boardView;
            _cubeMover = cubeMover;
            _wallTileConfig = wallTileConfig;
        }

        public void Load()
        {
            var config = _selection.SelectedLevel;
            _boardView.Initialize(config.GridSize, _wallTileConfig, config.ExitPositions);

            var spawnDict = new Dictionary<Vector2Int, CubeType>();
            foreach (var spawn in config.Cubes)
                spawnDict[spawn.Position] = spawn.Type;

            List<CubeView> spawnedCubes = new();

            for (int y = 0; y < config.GridSize.y; y++)
            {
                for (int x = 0; x < config.GridSize.x; x++)
                {
                    var pos = new Vector2Int(x, y);
                    if (spawnDict.TryGetValue(pos, out var type))
                    {
                        var cube = _factory.Create(type, pos);
                        if (cube != null)
                            spawnedCubes.Add(cube);
                    }
                }
            }

            var playerPos = config.PlayerStart;
            if (!spawnDict.ContainsKey(playerPos))
            {
                var playerCube = _factory.Create(CubeType.Player, playerPos);
                if (playerCube != null)
                    spawnedCubes.Add(playerCube);
            }

            _cubeMover.RegisterCubes(spawnedCubes);

            Debug.Log($"[LevelLoader] Всего кубиков заспавнено: {spawnedCubes.Count}");
        }
    }
}
