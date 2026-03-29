using System.Collections.Generic;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.UI.Base;
using _Scripts._Infrastructure.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game
{
    public class LevelSelectionPanel : AnimatedPanel
    {
        [Header("Level Buttons")]
        [SerializeField] private LevelButton[] levelButtons;
        
        [Header("Navigation")]
        [SerializeField] private Button previousPageButton;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private Button menuButton;
        
        private LevelsCatalog _levelsCatalog;
        private LevelSelectionService _levelSelectionService;
        private SceneLoader _sceneLoader;
        private AudioService _audioService;
        private UIPanelService _uiPanelService;

        private int _currentPage = 0;
        private int _levelsPerPage;
        private int _totalLevels = 0;
        private int _maxPage = 0;
        private bool _initialized = false;
        
        [Inject]
        public void Construct(
            LevelsCatalog levelsCatalog, 
            LevelSelectionService levelSelectionService, 
            SceneLoader sceneLoader,
            AudioService audioService,
            UIPanelService uiPanelService)
        {
            _levelsCatalog = levelsCatalog;
            _levelSelectionService = levelSelectionService;
            _sceneLoader = sceneLoader;
            _audioService = audioService;
            _uiPanelService = uiPanelService;
            
            InitializePanel();
        }

        protected override void Awake()
        {
            base.Awake();
            
            if (levelButtons == null || levelButtons.Length == 0)
            {
                Debug.LogError("LevelSelectionPanel: No level buttons assigned!");
            }
        }

        private void InitializePanel()
        {
            if (_initialized || levelButtons == null || levelButtons.Length == 0 || _levelsCatalog == null)
                return;
            
            _levelsPerPage = levelButtons.Length;
            _totalLevels = _levelsCatalog.Levels.Count;
            _maxPage = Mathf.CeilToInt((float)_totalLevels / _levelsPerPage) - 1;
            
            SetupButtons();
            _initialized = true;
        }

        private void SetupButtons()
        {
            previousPageButton.onClick.AddListener(OnPreviousPageClicked);
            nextPageButton.onClick.AddListener(OnNextPageClicked);
            menuButton.onClick.AddListener(OnMenuButtonClicked);

            for (int i = 0; i < levelButtons.Length; i++)
            {
                int buttonIndex = i; 
                levelButtons[i].GetComponent<Button>().onClick.AddListener(() => OnLevelButtonClicked(buttonIndex));
            }
        }

        private void OnDestroy()
        {
            previousPageButton.onClick.RemoveListener(OnPreviousPageClicked);
            nextPageButton.onClick.RemoveListener(OnNextPageClicked);
            menuButton.onClick.RemoveListener(OnMenuButtonClicked);

            for (int i = 0; i < levelButtons.Length; i++)
            {
                if (levelButtons[i] != null)
                    levelButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }

        public override void Open()
        {
            base.Open();
            
            if (!_initialized)
                InitializePanel();
                
            _currentPage = 0;
            UpdateLevelButtons();
        }

        private void UpdateLevelButtons()
        {
            if (!_initialized)
                return;
                
            int startLevel = _currentPage * _levelsPerPage;
            
            for (int i = 0; i < levelButtons.Length; i++)
            {
                int levelIndex = startLevel + i;
                
                if (levelIndex < _totalLevels)
                {
                    levelButtons[i].gameObject.SetActive(true);
                    levelButtons[i].SetLevelIndex(levelIndex);
                    
                    bool isUnlocked = IsLevelUnlocked(levelIndex);
                    levelButtons[i].SetUnlocked(isUnlocked);
                }
                else
                {
                    levelButtons[i].gameObject.SetActive(false);
                }
            }
            
            previousPageButton.interactable = _currentPage > 0;
            nextPageButton.interactable = _currentPage < _maxPage;
        }

        private bool IsLevelUnlocked(int levelIndex)
        {
            if (levelIndex == 0) return true;
            
            return PlayerPrefs.GetInt("Level_" + (levelIndex - 1) + "_Completed", 0) == 1;
        }

        private void OnLevelButtonClicked(int buttonIndex)
        {
            if (!_initialized)
                return;
                
            int levelIndex = _currentPage * _levelsPerPage + buttonIndex;
            
            if (levelIndex < _totalLevels && IsLevelUnlocked(levelIndex))
            {
                _audioService.PlaySound(SoundType.ButtonClick);
                
                LevelConfig selectedLevel = _levelsCatalog.GetLevel(levelIndex);
                if (selectedLevel != null)
                {
                    _levelSelectionService.SetSelectedLevel(selectedLevel, levelIndex);
                    _sceneLoader.Load("LevelScene");
                }
            }
        }

        private void OnPreviousPageClicked()
        {
            if (!_initialized)
                return;
                
            if (_currentPage > 0)
            {
                _audioService.PlaySound(SoundType.ButtonClick);
                _currentPage--;
                UpdateLevelButtons();
            }
        }

        private void OnNextPageClicked()
        {
            if (!_initialized)
                return;
                
            if (_currentPage < _maxPage)
            {
                _audioService.PlaySound(SoundType.ButtonClick);
                _currentPage++;
                UpdateLevelButtons();
            }
        }

        private void OnMenuButtonClicked()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            Close(() => _uiPanelService.Open(PanelType.Menu));
        }
    }
}
