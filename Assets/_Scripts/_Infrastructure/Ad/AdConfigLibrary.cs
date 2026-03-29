using System.Collections.Generic;
using UnityEngine;

namespace _Scripts._Infrastructure.Ad
{
    [CreateAssetMenu(fileName = "AdConfigLibrary", menuName = "Configs/Ad Config Library")]
    public class AdConfigLibrary : ScriptableObject
    {
        public List<ScriptableObject> Configs;

        public IAdConfig GetConfigFor(AdProviderType type)
        {
            foreach (var config in Configs)
            {
                if (config is IAdConfig adConfig && adConfig.ProviderType == type)
                    return adConfig;
            }

            return null;
        }
    }
}