using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Game
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform cellContainerPrefab;

        private const float DefaultAnchor = 0.5f;
        private const int BorderSize = 1;

        private GridLayoutGroup _layout;
        private RectTransform _rectTransform;
        private Vector2Int _gridSize;
        private readonly Dictionary<Vector2Int, CellData> _cellMap = new();
        private readonly Dictionary<RectTransform, Image> _imageCache = new();

        public float CellVisualSize => _layout.cellSize.x;

        private void Awake()
        {
            _layout = GetComponent<GridLayoutGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void Clear()
        {
            if (_cellMap == null) return;

            foreach (var cellData in _cellMap.Values)
            {
                if (cellData.View != null)
                    Destroy(cellData.View.gameObject);
            }

            _cellMap.Clear();
            _imageCache.Clear();
        }

        public void Initialize(Vector2Int size, WallTileConfig wallTileConfig, List<Vector2Int> exits)
        {
            if (wallTileConfig == null)
                throw new System.ArgumentNullException(nameof(wallTileConfig));
            if (exits == null)
                throw new System.ArgumentNullException(nameof(exits));

            _gridSize = size;
            _cellMap.Clear();
            _imageCache.Clear();

            SetupGridLayout(size);
            CreateCells(size, wallTileConfig, exits);
        }

        public bool TryGetCell(Vector2Int pos, out RectTransform cell, out CellType type)
        {
            if (_cellMap.TryGetValue(pos, out var data))
            {
                cell = data.View;
                type = data.Type;
                return true;
            }

            cell = null;
            type = CellType.Wall;
            return false;
        }

        public void UpdateBoard(WallTileConfig wallTileConfig, List<Vector2Int> exits)
        {
            if (wallTileConfig == null || exits == null)
                return;

            foreach (var pair in _cellMap)
            {
                Vector2Int pos = pair.Key;
                CellData data = pair.Value;

                bool isPlayable = IsPlayableCell(pos.x, pos.y, _gridSize);
                if (!isPlayable)
                {
                    UpdateCellVisuals(pos, data.View, wallTileConfig, exits);
                }
            }
        }

        private void SetupGridLayout(Vector2Int size)
        {
            int cols = size.x + BorderSize * 2;
            int rows = size.y + BorderSize * 2;

            var (cellWidth, cellHeight) = CalculateCellSize(_rectTransform, cols, rows);

            _layout.cellSize = new Vector2(cellWidth, cellHeight);
            _layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _layout.constraintCount = cols;
        }

        private (float width, float height) CalculateCellSize(RectTransform boardRect, int cols, int rows)
        {
            float totalWidth = boardRect.rect.width;
            float totalHeight = boardRect.rect.height;

            var pad = _layout.padding;
            var space = _layout.spacing;

            float cellW = (totalWidth - pad.left - pad.right - space.x * (cols - 1)) / cols;
            float cellH = (totalHeight - pad.top - pad.bottom - space.y * (rows - 1)) / rows;

            return (cellW, cellH);
        }

        private void CreateCells(Vector2Int size, WallTileConfig wallTileConfig, List<Vector2Int> exits)
        {
            int totalCells = (size.x + 2) * (size.y + 2);
            _cellMap.EnsureCapacity(totalCells);
            _imageCache.EnsureCapacity(totalCells);

            for (int y = size.y; y >= -1; y--)
            {
                for (int x = -1; x <= size.x; x++)
                {
                    Vector2Int pos = new(x, y);
                    var cell = CreateCell(pos);

                    bool isPlayable = IsPlayableCell(x, y, size);
                    SetupCell(pos, cell, isPlayable, wallTileConfig, exits);
                }
            }
        }

        private RectTransform CreateCell(Vector2Int pos)
        {
            RectTransform cell = Instantiate(cellContainerPrefab, transform);
            cell.name = $"Cell_{pos.x}_{pos.y}";
            SetupCellTransform(cell);

            Image img = cell.GetComponent<Image>();
            if (img != null)
            {
                _imageCache[cell] = img;
            }

            return cell;
        }

        private void SetupCellTransform(RectTransform cell)
        {
            cell.anchorMin = cell.anchorMax = new Vector2(DefaultAnchor, DefaultAnchor);
            cell.pivot = new Vector2(DefaultAnchor, DefaultAnchor);
            cell.anchoredPosition = Vector2.zero;
            cell.localScale = Vector3.one;
        }

        private bool IsPlayableCell(int x, int y, Vector2Int size) =>
            x >= 0 && x < size.x && y >= 0 && y < size.y;

        private void SetupCell(Vector2Int pos, RectTransform cell, bool isPlayable,
            WallTileConfig wallTileConfig, List<Vector2Int> exits)
        {
            if (isPlayable)
            {
                _cellMap[pos] = new CellData(cell, CellType.Empty);
                return;
            }

            Sprite sprite = wallTileConfig.GetWallSprite(pos, _gridSize, exits);

            if (_imageCache.TryGetValue(cell, out var img))
            {
                img.sprite = sprite;
                img.enabled = sprite != null;
                img.color = Color.white;
            }

            CellType type = DetermineCellType(sprite, pos, exits);
            _cellMap[pos] = new CellData(cell, type);
        }

        private CellType DetermineCellType(Sprite sprite, Vector2Int pos, List<Vector2Int> exits) =>
            sprite == null ? CellType.Empty :
            exits.Contains(pos) ? CellType.Exit : CellType.Wall;

        private void UpdateCellVisuals(Vector2Int pos, RectTransform cell,
            WallTileConfig wallTileConfig, List<Vector2Int> exits)
        {
            if (!_imageCache.TryGetValue(cell, out var img))
                return;

            Sprite sprite = wallTileConfig.GetWallSprite(pos, _gridSize, exits);
            img.sprite = sprite;
            img.enabled = sprite != null;

            CellType type = DetermineCellType(sprite, pos, exits);
            _cellMap[pos] = new CellData(cell, type);
        }
    }
}