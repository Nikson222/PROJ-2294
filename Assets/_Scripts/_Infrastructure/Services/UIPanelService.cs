using System;
using System.Collections.Generic;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.UI.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using static UnityEngine.Object;

namespace _Scripts._Infrastructure.Services
{
    public class UIPanelService : IDisposable
    {
        private readonly DiContainer _container;
        private readonly Transform _canvasTransform;

        private readonly Dictionary<PanelType, GameObject> _prefabs = new();
        private readonly Dictionary<PanelType, IPanel> _instances = new();
        private readonly List<GameObject> _instantiatedObjects = new();

        private readonly UIPanelConfig _config;

        public UIPanelService(DiContainer container, [Inject(Id = "UICanvas")] Canvas canvas, UIPanelConfig config)
        {
            _container = container;
            _canvasTransform = canvas.transform;
            _config = config;

            InitializeForScene(SceneManager.GetActiveScene().name);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void Open(PanelType type)
        {
            if (_instances.TryGetValue(type, out var panel))
            {
                panel.Open();
            }
            else
            {
                Debug.LogError($"UIPanelService: Panel {type} not found for current scene.");
            }
        }
        
        public void OpenWithParams<T>(PanelType type, T parameters)
        {
            if (_instances.TryGetValue(type, out var panel))
            {
                if (panel is IParameterizedPanel<T> parameterizedPanel)
                {
                    parameterizedPanel.SetParameters(parameters);
                    parameterizedPanel.Open();
                }
                else
                {
                    Debug.LogError($"UIPanelService: Panel {type} does not implement IParameterizedPanel<{typeof(T).Name}>");
                    panel.Open();
                }
            }
            else
            {
                Debug.LogError($"UIPanelService: Panel {type} not found for current scene.");
            }
        }

        public void Close(PanelType type)
        {
            if (_instances.TryGetValue(type, out var panel))
            {
                panel.Close();
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Cleanup();
            InitializeForScene(scene.name);
        }

        private void InitializeForScene(string sceneName)
        {
            foreach (var entry in _config.PanelEntries)
            {
                if (!entry.SceneName.Equals(sceneName, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (entry.PanelPrefab == null)
                {
                    Debug.LogWarning($"UIPanelService: Missing prefab for {entry.PanelType}");
                    continue;
                }

                var go = _container.InstantiatePrefab(entry.PanelPrefab, _canvasTransform);
                _container.InjectGameObject(go);

                if (!go.TryGetComponent<IPanel>(out var panel))
                {
                    Debug.LogError($"UIPanelService: {entry.PanelType} does not implement IPanel");
                    continue;
                }

                go.SetActive(entry.IsInitiallyOpen);
                if (entry.IsInitiallyOpen)
                    panel.Open();
                else
                    panel.Close();

                _instances[entry.PanelType] = panel;
                _prefabs[entry.PanelType] = entry.PanelPrefab;
                _instantiatedObjects.Add(go);
            }
        }

        private void Cleanup()
        {
            foreach (var go in _instantiatedObjects)
            {
                if (go != null)
                    Destroy(go);
            }

            _instantiatedObjects.Clear();
            _instances.Clear();
            _prefabs.Clear();
        }


        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Cleanup();
        }
    }
}
