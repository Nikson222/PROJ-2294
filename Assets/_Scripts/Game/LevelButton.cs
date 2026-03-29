using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Game
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private Image _lockImage;
        [SerializeField] private Text _levelNumText;
        
        public void SetLevelIndex(int index)
        {
            _levelNumText.text = (index + 1).ToString();
        }

        public void SetUnlocked(bool unlocked)
        {
            _lockImage.gameObject.SetActive(!unlocked);
        }
    }
}