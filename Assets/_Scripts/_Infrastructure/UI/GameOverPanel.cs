using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts.Game;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.UI.Base;
using _Scripts.Game.Services;

namespace _Scripts._Infrastructure.UI
{
    public class GameOverPanel : AnimatedPanel, IParameterizedPanel<GameOverPanelParams>
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Text _timeText;
        [SerializeField] private Text _swipesText;
        [SerializeField] private string _menuSceneName = "MenuScene";

        private SceneLoader _scenes;
        private AudioService _audio;
        private LevelsCatalog _levelsCatalog;
        private LevelSelectionService _levelSelectionService;
        private GameOverPanelParams _params;

        [Inject]
        public void Construct(
            SceneLoader loader, 
            AudioService audio, 
            LevelsCatalog levelsCatalog, 
            LevelSelectionService levelSelectionService)
        {
            _scenes = loader;
            _audio = audio;
            _levelsCatalog = levelsCatalog;
            _levelSelectionService = levelSelectionService;
        }

        private void Start()
        {
            _nextLevelButton.onClick.AddListener(OnClickNextLevel);
            _menuButton.onClick.AddListener(OnClickMenu);
        }

        private void OnDestroy()
        {
            if (_nextLevelButton != null) _nextLevelButton.onClick.RemoveAllListeners();
            if (_menuButton != null) _menuButton.onClick.RemoveAllListeners();
        }

        public void SetParameters(GameOverPanelParams parameters)
        {
            _params = parameters;
        }

        public override void Open()
        {
            if (_params != null)
            {
                int minutes = Mathf.FloorToInt(_params.CompletionTime / 60f);
                int seconds = Mathf.FloorToInt(_params.CompletionTime % 60f);
                if (_timeText != null)
                    _timeText.text = $"{minutes:00}:{seconds:00}";

                if (_swipesText != null)
                    _swipesText.text = $"{_params.SwipeCount}";
                    
                if (_nextLevelButton != null)
                    _nextLevelButton.gameObject.SetActive(_params.NextLevelAvailable);
            }
            
            base.Open();
        }

        private void OnClickNextLevel()
        {
            _audio.PlaySound(SoundType.ButtonClick);
            
            int nextLevelIndex = _params.CurrentLevelIndex + 1;
            LevelConfig nextLevel = _levelsCatalog.GetLevel(nextLevelIndex);
            
            if (nextLevel != null)
            {
                _levelSelectionService.SetSelectedLevel(nextLevel, nextLevelIndex);
                _audio.StopMusic();
                _scenes.Load("LevelScene", () => _audio.PlayMusic());
            }
            else
            {
                OnClickMenu();
            }
        }

        private void OnClickMenu()
        {
            _audio.PlaySound(SoundType.ButtonClick);
            _audio.StopMusic();
            _scenes.Load(_menuSceneName, () => _audio.PlayMusic());
        }
    }
}