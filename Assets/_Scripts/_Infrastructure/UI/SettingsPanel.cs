using _Scripts._Infrastructure.Configs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.UI.Base;

namespace _Scripts._Infrastructure.UI
{
    public class SettingsPanel : AnimatedPanel
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Slider _soundSlider;
        [SerializeField] private Slider _musicSlider;

        private AudioService _audio;
        private UIPanelService _ui;

        private float _lastSoundVolume = 0.5f;
        private float _lastMusicVolume = 0.5f;
        private const float VolumeThreshold = 0.25f;

        [Inject]
        public void Construct(AudioService audioService, UIPanelService uiPanelService)
        {
            _audio = audioService;
            _ui = uiPanelService;
        }

        private void Start()
        {
            _soundSlider.onValueChanged.AddListener(SetSound);
            _musicSlider.onValueChanged.AddListener(SetMusic);
            _closeButton.onClick.AddListener(() => Close());

            _soundSlider.value = _audio.SoundVolume;
            _musicSlider.value = _audio.MusicVolume;

            _lastSoundVolume = _audio.SoundVolume;
            _lastMusicVolume = _audio.MusicVolume;
        }

        private void SetSound(float volume)
        {
            if (Mathf.Abs(volume - _lastSoundVolume) >= VolumeThreshold)
            {
                _audio.PlaySound(SoundType.Slider);
                _lastSoundVolume = volume;
            }

            _audio.SoundVolume = volume;
        }

        private void SetMusic(float volume)
        {
            if (Mathf.Abs(volume - _lastMusicVolume) >= VolumeThreshold)
            {
                _audio.PlaySound(SoundType.Slider);
                _lastMusicVolume = volume;
            }

            _audio.MusicVolume = volume;
        }

        public override void Close(System.Action onClosed = null)
        {
            _audio.PlaySound(SoundType.ButtonClick);
            base.Close(() => _ui.Open(PanelType.Menu));
        }
    }
}
