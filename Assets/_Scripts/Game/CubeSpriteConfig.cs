using System.Linq;
using UnityEngine;

namespace _Scripts.Game
{
    [CreateAssetMenu(fileName = "CubeSpriteConfig", menuName = "Game/Cube Sprite Config")]
    public class CubeSpriteConfig : ScriptableObject
    {
        public CubeSpriteEntry[] Entries;

        public Sprite GetSprite(CubeType type) =>
            Entries.FirstOrDefault(e => e.Type == type)?.Sprite;
    }

    [System.Serializable]
    public class CubeSpriteEntry
    {
        public CubeType Type;
        public Sprite Sprite;
    }
}
