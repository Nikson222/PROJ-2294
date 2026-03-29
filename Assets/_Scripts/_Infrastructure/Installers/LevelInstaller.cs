using _Scripts._Infrastructure.Extensions;
using UnityEngine;
using Zenject;
using _Scripts._Infrastructure.StateMachine;
using _Scripts.Game;
using _Scripts.Game.Services;

namespace _Scripts.Installers
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private BoardView boardView;
        [SerializeField] private CubeView cubePrefab;
        [SerializeField] private CubeSpriteConfig spriteConfig;
        [SerializeField] private Transform cubeContainer;
        [SerializeField] private WallTileConfig wallTileConfig;

        public override void InstallBindings()
        {
            Container.Bind<LevelState>().AsSingle();
            Container.Bind<GoalTracker>().AsSingle();
            Container.Bind<CubeFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
            Container.Bind<LevelLoader>().AsSingle();
            Container.Bind<LevelSelectionService>().AsSingle();

            Container.Bind<BoardView>().FromInstance(boardView).AsSingle();
            Container.Bind<CubeSpriteConfig>().FromInstance(spriteConfig).AsSingle();
            Container.Bind<CubeView>().FromInstance(cubePrefab).AsSingle();
            Container.Bind<Transform>().WithId("CubeContainer").FromInstance(cubeContainer).AsSingle();
            Container.Bind<WallTileConfig>().FromInstance(wallTileConfig).AsSingle();

            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<CubeMover>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStatsTracker>().AsSingle();

            Container.BindAndRegisterState<LoadLevelState, GameStateMachine>();
            Container.BindAndRegisterState<GameplayState, GameStateMachine>();
            Container.BindAndRegisterState<WinState, GameStateMachine>();
            Container.BindAndRegisterState<LoseState, GameStateMachine>();
        }
    }
}
