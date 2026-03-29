using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class LevelSelectionService
    {
        private const string SelectedLevelIndexKey = "SelectedLevelIndex";
        
        [Inject] private LevelsCatalog _levelsCatalog;
        
        public LevelConfig SelectedLevel { get; private set; }
        public int SelectedLevelIndex { get; private set; }
        
        [Inject]
        public void Initialize()
        {
            int savedIndex = PlayerPrefs.GetInt(SelectedLevelIndexKey, 0);
            if (_levelsCatalog != null)
            {
                var level = _levelsCatalog.GetLevel(savedIndex);
                if (level != null)
                {
                    SelectedLevel = level;
                    SelectedLevelIndex = savedIndex;
                }
            }
        }

        public void SetSelectedLevel(LevelConfig config, int levelIndex)
        {
            SelectedLevel = config;
            SelectedLevelIndex = levelIndex;
            
            PlayerPrefs.SetInt(SelectedLevelIndexKey, levelIndex);
            PlayerPrefs.Save();
        }
    }
}