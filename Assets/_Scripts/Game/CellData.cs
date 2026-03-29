using UnityEngine;

namespace _Scripts.Game
{
    public class CellData
    {
        public RectTransform View;
        public CellType Type;

        public CellData(RectTransform view, CellType type)
        {
            View = view;
            Type = type;
        }
    }
}