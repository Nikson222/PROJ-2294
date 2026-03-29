using System.Linq;
using UnityEngine;

namespace _Scripts.Game
{
    [CreateAssetMenu(fileName = "CubeColorConfig", menuName = "Game/Cube Color Config")]
    public class CubeColorConfig : ScriptableObject
    {
        public CubeColorEntry[] Entries;

        public Color GetColor(CubeType type) =>
            Entries.FirstOrDefault(e => e.Type == type)?.Color ?? Color.white;
    }

    [System.Serializable]
    public class CubeColorEntry
    {
        public CubeType Type;
        public Color Color;
    }
}