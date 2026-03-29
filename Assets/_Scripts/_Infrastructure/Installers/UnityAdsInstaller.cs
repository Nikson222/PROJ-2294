using _Scripts._Infrastructure.Ad;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class AdInstaller : MonoInstaller
    {
        [SerializeField] private AdProviderType _selectedProvider;
        [SerializeField] private AdConfigLibrary _configLibrary;

        public override void InstallBindings()
        {
            var config = _configLibrary.GetConfigFor(_selectedProvider);

            switch (_selectedProvider)
            {
                case AdProviderType.Unity:
                    var unityConfig = config as UnityAdsConfig;
                    Container.BindInstance(unityConfig).AsSingle();
                    Container.Bind<IAdService>().To<UnityAdService>().AsSingle().NonLazy();
                    break;

                default:
                    throw new System.Exception("Unknown ad provider selected.");
            }
        }
    }
}