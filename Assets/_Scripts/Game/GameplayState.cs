using UnityEngine;
using _Scripts._Infrastructure.StateMachine;
using _Scripts.Game.Services;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.Configs;

namespace _Scripts.Game
{
    public class GameplayState : IState
    {
        private readonly InputService _input;
        private readonly CubeMover _mover;
        private readonly GoalTracker _goalTracker;
        private readonly GameStateMachine _stateMachine;
        private readonly GameStatsTracker _statsTracker;
        private readonly AudioService _audioService;

        private bool _gameOver;

        public GameplayState(
            InputService input,
            CubeMover mover,
            GoalTracker goalTracker,
            GameStateMachine stateMachine,
            GameStatsTracker statsTracker,
            AudioService audioService)
        {
            _input = input;
            _mover = mover;
            _goalTracker = goalTracker;
            _stateMachine = stateMachine;
            _statsTracker = statsTracker;
            _audioService = audioService;
        }

        public void Enter()
        {
            _gameOver = false;
            _input.OnSwipe += OnSwipe;
            _mover.OnPlayerEnteredDoor += OnPlayerEnteredDoor;
            _mover.OnCubeEnteredDoor += CheckGameStatusAfterMove;
            
            // Сбросим статистику при входе в состояние игры
            _statsTracker.StartTracking();
        }

        public void Exit()
        {
            _input.OnSwipe -= OnSwipe;
            _mover.OnPlayerEnteredDoor -= OnPlayerEnteredDoor;
            _mover.OnCubeEnteredDoor -= CheckGameStatusAfterMove;
            
            // Останавливаем отслеживание статистики при выходе из состояния
            _statsTracker.StopTracking();
        }

        private void OnSwipe(Vector2Int dir)
        {
            // Записываем свайп в статистику
            _statsTracker.RecordSwipe();
            
            // Воспроизводим звук свайпа
            _audioService.PlaySound(SoundType.Swipe);
            
            _mover.MovePlayer(dir);
        }

        private void OnPlayerEnteredDoor()
        {
            if (_gameOver) return;

            _gameOver = true;
            _stateMachine.Enter<LoseState>();
        }

        private void CheckGameStatusAfterMove()
        {
            if (_gameOver) return;

            if (_goalTracker.IsLevelComplete())
            {
                _gameOver = true;
                _stateMachine.Enter<WinState>();
            }
            else if (_goalTracker.IsGameLost())
            {
                _gameOver = true;
                _stateMachine.Enter<LoseState>();
            }
        }
    }
}