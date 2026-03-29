using UnityEngine;
using Zenject;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.StateMachine;
using _Scripts.Game;
using _Scripts.Game.Services;
using LoadingCurtain;

namespace _Scripts._Infrastructure.Installers
{
    public class GameProjectInstaller : MonoInstaller
    {
        [SerializeField] private CoroutineRunner _coroutineRunnerPrefab;
        [SerializeField] private Canvas _curtainCanvas;
        [SerializeField] private LevelsCatalog _levelsCatalog;

        public override void InstallBindings()
        {
            var runner = Container.InstantiatePrefabForComponent<CoroutineRunner>(_coroutineRunnerPrefab);
            Container.BindInterfacesAndSelfTo<CoroutineRunner>().FromInstance(runner).AsSingle();

            var curtainObj = Container.InstantiatePrefab(_curtainCanvas);
            var curtain = curtainObj.GetComponentInChildren<Curtain>();
            Container.BindInterfacesAndSelfTo<Curtain>().FromInstance(curtain).AsSingle();

            Container.Bind<LevelsCatalog>().FromInstance(_levelsCatalog).AsSingle();
            Container.Bind<LevelSelectionService>().AsSingle();

            Container.BindInterfacesAndSelfTo<SaveLoadService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerProfile>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreCounter>().AsSingle();
        }
    }
}