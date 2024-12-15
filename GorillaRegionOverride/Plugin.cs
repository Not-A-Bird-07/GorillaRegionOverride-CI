using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GorillaRegionOverride.Behaviours;
using UnityEngine;

namespace GorillaRegionOverride
{
    [BepInDependency("dev.gorillacomputer", "1.0.0")]
    [BepInPlugin(Constants.Guid, Constants.Name, Constants.Version)]
    public class Plugin : BaseUnityPlugin
    {
        /// <summary>
        /// The ManualLogSource tied to this plugin
        /// </summary>
        public static ManualLogSource TiedLogSource;

        /// <summary>
        /// The ConfigFile tied to this plugin
        /// </summary>
        public static ConfigFile TiedConfigFile;

        public Plugin()
        {
            TiedLogSource = Logger;
            TiedConfigFile = Config;
            GorillaTagger.OnPlayerSpawned(() => new GameObject(typeof(Main).FullName).AddComponent<Main>());
        }
    }
}
