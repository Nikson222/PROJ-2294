using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        public Vector2Int GridSize;
        public Vector2Int PlayerStart;
        public List<CubeSpawnData> Cubes;
        public List<Vector2Int> ExitPositions;
        public int Index { get; set; }
    }
}