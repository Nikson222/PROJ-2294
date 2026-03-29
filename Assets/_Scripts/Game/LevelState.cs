using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game
{
    public class LevelState
    {
        private readonly Dictionary<Vector2Int, CubeView> _cubeMap = new();

        public int Count => _cubeMap.Count;

        public void AddCube(CubeView cube)
        {
            _cubeMap[cube.GridPosition] = cube;
        }

        public CubeView GetCubeAt(Vector2Int pos)
        {
            _cubeMap.TryGetValue(pos, out var cube);
            return cube;
        }

        public void MoveCube(Vector2Int from, Vector2Int to, CubeView cube)
        {
            if (_cubeMap.ContainsKey(from))
                _cubeMap.Remove(from);

            _cubeMap[to] = cube;
        }

        public void RemoveCube(CubeView cube)
        {
            if (_cubeMap.ContainsKey(cube.GridPosition))
                _cubeMap.Remove(cube.GridPosition);
        }

        public void Clear()
        {
            _cubeMap.Clear();
        }
    }
}