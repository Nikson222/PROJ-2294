using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.Configs;
using UnityEngine;

namespace _Scripts.Game
{
    public class CubeMover
    {
        private readonly CoroutineRunner _runner;
        private readonly BoardView _boardView;
        private readonly LevelSelectionService _selection;
        private readonly LevelState _state;
        private readonly GoalTracker _goalTracker;
        private readonly AudioService _audioService;

        private CubeView _playerCube;

        public bool IsMoving { get; private set; }

        public event Action OnPlayerEnteredDoor = delegate { };
        public event Action OnCubeEnteredDoor = delegate { };

        public CubeMover(
            CoroutineRunner runner,
            BoardView boardView,
            LevelSelectionService selection,
            LevelState state,
            GoalTracker goalTracker,
            AudioService audioService)
        {
            _runner = runner;
            _boardView = boardView;
            _selection = selection;
            _state = state;
            _goalTracker = goalTracker;
            _audioService = audioService;
        }

        public void RegisterCubes(IEnumerable<CubeView> cubes)
        {
            _state.Clear();
            
            var cubesList = cubes.ToList();
            _goalTracker.Init(cubesList);

            foreach (var cube in cubesList)
            {
                _state.AddCube(cube);
                cube.OnMoved += HandleCubeMoved;

                if (cube.Type == CubeType.Player)
                    _playerCube = cube;
            }
        }

        public void MovePlayer(Vector2Int direction)
        {
            if (IsMoving || _playerCube == null || _playerCube.IsMoving)
                return;

            _runner.StartCoroutine(MoveUntilBlocked(_playerCube, direction));
        }

        private IEnumerator MoveUntilBlocked(CubeView cube, Vector2Int dir)
        {
            IsMoving = true;

            Vector2Int current = cube.GridPosition;
            Vector2Int next = current + dir;

            while (IsInsideBoard(next))
            {
                if (!_boardView.TryGetCell(next, out _, out var cellType))
                    break;
                
                if (_state.GetCubeAt(next) != null)
                    break;

                if (cellType == CellType.Wall)
                    break;

                current = next;

                if (cellType == CellType.Exit)
                {
                    if (_boardView.TryGetCell(current, out var doorCell, out _))
                    {
                        yield return cube.MoveTo(current, doorCell);
                        _goalTracker.MarkInGoal(cube);

                        if (cube.Type == CubeType.Player)
                        {
                            yield return cube.ScaleDownAndDestroy();
                            OnPlayerEnteredDoor?.Invoke();
                        }
                        else
                        {
                            _state.RemoveCube(cube);
                            yield return cube.ScaleDownAndDestroy();
                            OnCubeEnteredDoor?.Invoke();
                        }
                        
                        IsMoving = false;
                        yield break;
                    }
                }

                next += dir;
            }

            if (!IsInsideBoard(next) || 
                !_boardView.TryGetCell(next, out _, out var nextCellType) ||
                nextCellType == CellType.Wall)
            {
                // Столкновение со стеной
                _audioService.PlaySound(SoundType.Collision);
                
                if (_boardView.TryGetCell(current, out var touchCell, out _))
                {
                    var offset = GetVisualTouchOffset(cube, dir, touchingWall: true);
                    yield return cube.MoveToWorldOffset(touchCell.position + offset);
                    yield return cube.MoveTo(current, touchCell);
                }
                
                IsMoving = false;
                yield break;
            }

            CubeView nextCube = _state.GetCubeAt(next);
            if (nextCube != null)
            {
                // Столкновение с другим кубиком
                _audioService.PlaySound(SoundType.Collision);
                
                if (_boardView.TryGetCell(current, out var currentCell, out _))
                {
                    var offset = GetVisualTouchOffset(cube, dir, touchingWall: false);
                    yield return cube.MoveToWorldOffset(currentCell.position + offset);
                }

                yield return MoveUntilBlocked(nextCube, dir);

                if (_state.GetCubeAt(next) == null)
                {
                    if (_boardView.TryGetCell(current, out var stopCell, out _))
                        yield return cube.MoveTo(current, stopCell);
                }
                else
                {
                    if (_boardView.TryGetCell(current, out var returnCell, out _))
                        yield return cube.MoveTo(current, returnCell);
                }
                
                IsMoving = false;
                yield break;
            }

            if (_boardView.TryGetCell(current, out var finalCell, out _))
                yield return cube.MoveTo(current, finalCell);

            IsMoving = false;
        }

        private Vector3 GetVisualTouchOffset(CubeView cube, Vector2Int direction, bool touchingWall)
        {
            float cellSize = _boardView.CellVisualSize;
            Vector2 cubeSize = cube.GetVisualDimensions();
            
            float availableSpace = (cellSize - Mathf.Max(cubeSize.x, cubeSize.y)) / 2f;
            float offset = touchingWall ? availableSpace : availableSpace * 2f;
            
            return new Vector3(direction.x * offset, direction.y * offset, 0f);
        }

        private void HandleCubeMoved(Vector2Int from, Vector2Int to, CubeView cube)
        {
            _state.MoveCube(from, to, cube);
        }

        private bool IsInsideBoard(Vector2Int pos)
        {
            var size = _selection.SelectedLevel?.GridSize ?? Vector2Int.zero;
            
            if (_boardView.TryGetCell(pos, out _, out var cellType) && cellType == CellType.Exit)
                return true;
                
            return pos.x >= 0 && pos.y >= 0 && pos.x < size.x && pos.y < size.y;
        }
    }
}
