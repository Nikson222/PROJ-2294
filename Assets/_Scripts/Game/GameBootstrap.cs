using UnityEngine;
using Zenject;
using _Scripts._Infrastructure.StateMachine;

namespace _Scripts.Game
{
    public class GameBootstrap : MonoBehaviour
    {
        private GameStateMachine _stateMachine;
        private LevelsCatalog _levelsCatalog;
        private LevelSelectionService _levelSelectionService;
        
        [Inject]
        public void Construct(GameStateMachine stateMachine, LevelsCatalog levelsCatalog, LevelSelectionService levelSelectionService)
        {
            _stateMachine = stateMachine;
            _levelsCatalog = levelsCatalog;
            _levelSelectionService = levelSelectionService;
        }

        private void Start()
        {
            _stateMachine.Enter<LoadLevelState>();
        }
    }
}