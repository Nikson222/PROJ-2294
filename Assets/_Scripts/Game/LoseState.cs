using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.StateMachine;
using Zenject;

namespace _Scripts.Game
{
    public class LoseState : IState
    {
        private readonly SceneLoader _sceneLoader;
        private readonly AudioService _audioService;

        [Inject]
        public LoseState(SceneLoader sceneLoader, AudioService audioService)
        {
            _sceneLoader = sceneLoader;
            _audioService = audioService;
        }

        public void Enter()
        {
            _audioService.PlaySound(SoundType.Lose);
            _sceneLoader.Load("LevelScene");
        }

        public void Exit() { }
    }
}