using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts._Infrastructure.Ad
{
    public class RewardedAdsButton : MonoBehaviour
    {
        [SerializeField] private Button _showAdButton;

        private IAdService _adService;
        public event Action OnRewardedSuccess;

        [Inject]
        public void Construct(IAdService adService)
        {
            _adService = adService;
        }

        private void Start()
        {
            _showAdButton.interactable = _adService.IsReady(AdType.Rewarded);
            _showAdButton.onClick.AddListener(OnClickShowAd);

            _adService.OnAdCompleted += OnAdCompleted;
        }

        private void OnClickShowAd()
        {
            _adService.ShowAd(AdType.Rewarded);
        }

        private void OnAdCompleted(AdType type)
        {
            if (type != AdType.Rewarded)
                return;

            OnRewardedSuccess?.Invoke();
        }

        private void OnDestroy()
        {
            _adService.OnAdCompleted -= OnAdCompleted;
        }
    }
}