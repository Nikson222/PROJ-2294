using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _canvasPrefab;
        [SerializeField] private UIPanelConfig _uiPanelConfig;

        public override void InstallBindings()
        {
            var canvas = Container.InstantiatePrefabForComponent<Canvas>(_canvasPrefab);
            DontDestroyOnLoad(canvas.gameObject);

            Container.Bind<Canvas>().WithId("UICanvas").FromInstance(canvas).AsSingle();
            Container.Bind<UIPanelService>().AsSingle().WithArguments(Container, canvas, _uiPanelConfig).NonLazy();
            Container.InjectGameObject(canvas.gameObject);
        }
    }
}