using _Scripts._Infrastructure.Configs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.UI.Base;
using _Scripts.Game;

namespace _Scripts._Infrastructure.UI
{
    public class MenuPanel : AnimatedPanel
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _gameCenterButton;
        [SerializeField] private Button _levelSelectionButton;

        private AudioService _audio;
        private UIPanelService _ui;
        private SceneLoader _scenes;
        private LevelsCatalog _levelsCatalog;
        private LevelSelectionService _levelSelectionService;

        private const string LastLevelKey = "LastLevelIndex";

        [Inject]
        public void Construct(
            SceneLoader sceneLoader, 
            AudioService audioService, 
            UIPanelService uiPanelService,
            LevelsCatalog levelsCatalog,
            LevelSelectionService levelSelectionService)
        {
            _scenes = sceneLoader;
            _audio = audioService;
            _ui = uiPanelService;
            _levelsCatalog = levelsCatalog;
            _levelSelectionService = levelSelectionService;
        }

        private void Start()
        {
            _audio.PlayMusic();

            _gameCenterButton.onClick.AddListener(() => KTGameCenter.SharedCenter().ShowAchievements());
            _playButton.onClick.AddListener(OnClickPlay);
            _settingsButton.onClick.AddListener(OnClickSettings);
            _exitButton.onClick.AddListener(Application.Quit);
            
            if (_levelSelectionButton != null)
                _levelSelectionButton.onClick.AddListener(OnClickLevelSelection);
        }

        private void OnDestroy()
        {
            if (_gameCenterButton != null) _gameCenterButton.onClick.RemoveAllListeners();
            if (_playButton != null) _playButton.onClick.RemoveAllListeners();
            if (_settingsButton != null) _settingsButton.onClick.RemoveAllListeners();
            if (_exitButton != null) _exitButton.onClick.RemoveAllListeners();
            if (_levelSelectionButton != null) _levelSelectionButton.onClick.RemoveAllListeners();
        }

        private void OnClickPlay()
        {
            _audio.PlaySound(SoundType.ButtonClick);
            
            int lastLevelIndex = PlayerPrefs.GetInt(LastLevelKey, 0);
            LevelConfig levelConfig = _levelsCatalog.GetLevel(lastLevelIndex);
            
            if (levelConfig == null)
            {
                lastLevelIndex = 0;
                levelConfig = _levelsCatalog.GetLevel(0);
            }
            
            _levelSelectionService.SetSelectedLevel(levelConfig, lastLevelIndex);
            
            _audio.StopMusic();
            _scenes.Load("LevelScene", () => _audio.PlayMusic());
        }

        private void OnClickLevelSelection()
        {
            _audio.PlaySound(SoundType.ButtonClick);
            Close(() => _ui.Open(PanelType.LevelSelection));
        }

        private void OnClickSettings()
        {
            _audio.PlaySound(SoundType.ButtonClick);
            Close(() => _ui.Open(PanelType.Settings));
        }
    }
}