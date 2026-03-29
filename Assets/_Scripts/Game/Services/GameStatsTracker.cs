using System;
using UnityEngine;
using Zenject;

namespace _Scripts.Game.Services
{
    public class GameStatsTracker : IInitializable, IDisposable, ITickable
    {
        private int _swipeCount;
        private float _gameTime;
        private bool _isTracking;
        private bool _gameStarted;
        
        public int SwipeCount => _swipeCount;
        public float GameTime => _gameTime;
        
        public void Initialize()
        {
            ResetStats();
        }
        
        public void Dispose()
        {
        }
        
        public void ResetStats()
        {
            _swipeCount = 0;
            _gameTime = 0;
            _isTracking = false;
            _gameStarted = false;
        }
        
        public void RecordSwipe()
        {
            _swipeCount++;
            
            if (!_gameStarted)
            {
                _gameStarted = true;
                _isTracking = true;
            }
        }
        
        public void StartTracking()
        {
            ResetStats();
        }
        
        public void StopTracking()
        {
            _isTracking = false;
        }
        
        public void Tick()
        {
            if (_isTracking && _gameStarted)
            {
                _gameTime += Time.deltaTime;
            }
        }
        
        public string GetFormattedTime()
        {
            int minutes = Mathf.FloorToInt(_gameTime / 60f);
            int seconds = Mathf.FloorToInt(_gameTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}
