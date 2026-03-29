using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Game
{
    [CreateAssetMenu(fileName = "LevelsCatalog", menuName = "Game/Levels Catalog")]
    public class LevelsCatalog : ScriptableObject
    {
        public List<LevelConfig> Levels;

        public LevelConfig GetLevel(int index)
        {
            if (index < 0 || index >= Levels.Count)
                return Levels.Last(); 

            return Levels[index];
        }
    }
}