using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game
{
    [CreateAssetMenu(fileName = "WallTileConfig", menuName = "Game/Wall Tile Config")]
    public class WallTileConfig : ScriptableObject
    {
        private const int BORDER_SIZE = 1;
    
        [Header("Стены")]
        public Sprite TopWall;
        public Sprite BottomWall;
        public Sprite LeftWall;
        public Sprite RightWall;

        [Header("Выходы")]
        public Sprite TopExit;
        public Sprite BottomExit;
        public Sprite LeftExit;
        public Sprite RightExit;

        [Header("Углы")]
        public Sprite CornerTL;
        public Sprite CornerTR;
        public Sprite CornerBL;
        public Sprite CornerBR;

        public Sprite GetWallSprite(Vector2Int pos, Vector2Int gridSize, List<Vector2Int> exits)
        {
            if (pos.x == -BORDER_SIZE && pos.y == gridSize.y) return CornerTL;
            if (pos.x == gridSize.x && pos.y == gridSize.y) return CornerTR;
            if (pos.x == -BORDER_SIZE && pos.y == -BORDER_SIZE) return CornerBL;
            if (pos.x == gridSize.x && pos.y == -BORDER_SIZE) return CornerBR;

            if (exits.Contains(pos))
            {
                if (pos.y == gridSize.y) return TopExit;
                if (pos.y == -BORDER_SIZE) return BottomExit;
                if (pos.x == -BORDER_SIZE) return LeftExit;
                if (pos.x == gridSize.x) return RightExit;
            }

            if (pos.y == gridSize.y) return TopWall;
            if (pos.y == -BORDER_SIZE) return BottomWall;
            if (pos.x == -BORDER_SIZE) return LeftWall;
            if (pos.x == gridSize.x) return RightWall;

            return null;
        }
    }
}