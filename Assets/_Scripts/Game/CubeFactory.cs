using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class CubeFactory
    {
        private readonly DiContainer container;
        private readonly CubeSpriteConfig spriteConfig;
        private readonly CubeView cubePrefab;
        private readonly BoardView boardView;

        public CubeFactory(
            DiContainer container,
            CubeSpriteConfig spriteConfig,
            CubeView cubePrefab,
            BoardView boardView)
        {
            this.container = container;
            this.spriteConfig = spriteConfig;
            this.cubePrefab = cubePrefab;
            this.boardView = boardView;
        }

        public CubeView Create(CubeType type, Vector2Int gridPos)
        {
            if (!boardView.TryGetCell(gridPos, out var parentCell, out _))
                return null;

            var cube = container.InstantiatePrefabForComponent<CubeView>(cubePrefab, parentCell);

            var rt = (RectTransform)cube.transform;
            float scaleFactor = 0.8f;

            rt.anchorMin = new Vector2((1 - scaleFactor) / 2f, (1 - scaleFactor) / 2f);
            rt.anchorMax = new Vector2((1 + scaleFactor) / 2f, (1 + scaleFactor) / 2f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Sprite sprite = spriteConfig.GetSprite(type);
            if (sprite == null)
            {
                Debug.LogWarning($"Спрайт для типа куба {type} не найден в конфигурации!");
            }

            cube.Initialize(type, sprite, gridPos);
            return cube;
        }
    }
}