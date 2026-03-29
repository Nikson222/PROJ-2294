using _Scripts._Infrastructure.StateMachine;

namespace _Scripts.Game
{
    public class LoadLevelState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly LevelSelectionService _selection;
        private readonly LevelLoader _loader;

        public LoadLevelState(
            GameStateMachine stateMachine,
            LevelSelectionService selection,
            LevelLoader loader)
        {
            _stateMachine = stateMachine;
            _selection = selection;
            _loader = loader;
        }

        public void Enter()
        {
            _loader.Load();
            _stateMachine.Enter<GameplayState>();
        }

        public void Exit() { }
    }
}