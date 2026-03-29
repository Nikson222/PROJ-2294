using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.StateMachine;
using _Scripts._Infrastructure.UI.Base;
using _Scripts.Game.Services;
using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class WinState : IState
    {
        private readonly UIPanelService _uiPanelService;
        private readonly AudioService _audioService;
        private readonly GameStatsTracker _statsTracker;
        private readonly LevelSelectionService _levelSelectionService;
        private readonly LevelsCatalog _levelsCatalog;

        [Inject]
        public WinState(
            UIPanelService uiPanelService, 
            AudioService audioService,
            GameStatsTracker statsTracker,
            LevelSelectionService levelSelectionService,
            LevelsCatalog levelsCatalog)
        {
            _uiPanelService = uiPanelService;
            _audioService = audioService;
            _statsTracker = statsTracker;
            _levelSelectionService = levelSelectionService;
            _levelsCatalog = levelsCatalog;
        }

        public void Enter()
        {
            int currentLevelIndex = _levelSelectionService.SelectedLevelIndex;
            
            PlayerPrefs.SetInt($"Level_{currentLevelIndex}_Completed", 1);
            
            PlayerPrefs.SetInt("LastLevelIndex", currentLevelIndex + 1);
            PlayerPrefs.Save();
            
            _statsTracker.StopTracking();
            
            _audioService.PlaySound(SoundType.Win);
            
            _uiPanelService.OpenWithParams(PanelType.GameOver, new GameOverPanelParams
            {
                CompletionTime = _statsTracker.GameTime,
                SwipeCount = _statsTracker.SwipeCount,
                CurrentLevelIndex = currentLevelIndex,
                NextLevelAvailable = currentLevelIndex < _levelsCatalog.Levels.Count - 1
            });
        }

        public void Exit() { }
    }
    
    public class GameOverPanelParams
    {
        public float CompletionTime;
        public int SwipeCount;
        public int CurrentLevelIndex;
        public bool NextLevelAvailable;
    }
}