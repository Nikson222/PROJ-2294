using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Game
{
    public class GoalTracker
    {
        private HashSet<CubeView> _allCubes;
        private HashSet<CubeView> _targetCubes;
        private HashSet<CubeView> _cubesInGoal;
        private CubeView _playerCube;

        public void Init(IEnumerable<CubeView> cubes)
        {
            _allCubes = new HashSet<CubeView>(cubes);
            _cubesInGoal = new HashSet<CubeView>();
            _playerCube = _allCubes.FirstOrDefault(c => c.Type == CubeType.Player);
            _targetCubes = _allCubes.Where(c => c.Type != CubeType.Player).ToHashSet();
        }

        public void MarkInGoal(CubeView cube)
        {
            if (cube != null)
            {
                _cubesInGoal.Add(cube);
            }
        }

        public bool IsLevelComplete()
        {
            return _targetCubes.Where(c => c != null).All(c => _cubesInGoal.Contains(c));
        }

        public bool IsPlayerInGoal()
        {
            return _playerCube != null && _cubesInGoal.Contains(_playerCube);
        }

        public bool HasMovableCubes()
        {
            return _allCubes.Any(c => c != null && !c.IsMoving);
        }

        public bool IsGameLost()
        {
            return IsPlayerInGoal() || !HasMovableCubes();
        }

        public void Reset()
        {
            _allCubes.Clear();
            _cubesInGoal.Clear();
            _targetCubes.Clear();
            _playerCube = null;
        }
    }
}
