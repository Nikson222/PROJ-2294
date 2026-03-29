using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game
{
    public class LevelHub : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _menuButton;
        
        private SceneLoader _sceneLoader;
        private AudioService _audioService;
        
        [Inject]
        public void Construct(
            SceneLoader sceneLoader, 
            AudioService audioService)
        {
            _sceneLoader = sceneLoader;
            _audioService = audioService;
        }
        
        private void Start()
        {
            SetupButtons();
        }
        
        private void OnDestroy()
        {
            if (_restartButton != null) _restartButton.onClick.RemoveAllListeners();
            if (_menuButton != null) _menuButton.onClick.RemoveAllListeners();
        }
        
        private void SetupButtons()
        {
            if (_restartButton != null)
                _restartButton.onClick.AddListener(OnRestartClicked);
                
            if (_menuButton != null)
                _menuButton.onClick.AddListener(OnMenuClicked);
        }
        
        private void OnRestartClicked()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _sceneLoader.Load("LevelScene");
        }
        
        private void OnMenuClicked()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _sceneLoader.Load("WhiteMenu");
        }
    }
}
