using GorillaRegionOverride.Models;
using GorillaRegionOverride.Tools;
using System;

namespace GorillaRegionOverride.Behaviours
{
    internal class Main : Singleton<Main>
    {
        private NetworkSystem NetSys => NetworkSystem.Instance;

        public Patching Patching = new(Constants.Guid);

        public Config Config = new(Plugin.TiedConfigFile);

        protected override void Initialize()
        {
            Logging.Info("Initialize");

            if (NetSys && !NetSys.GetComponent<NetworkSystemPUN>())
            {
                Logging.Warn("NetSys is not based on PUN backend - this mod won't have any effect on server region");
            }

            (bool overrideRegion, int region) = Data();
            Evaluate(overrideRegion, region);
        }

        public (bool overrideRegion, int region) Data() => (Config.OverrideRegion.Value, Config.Region.Value);

        public void Configure(bool overrideRegion, int region)
        {
            Logging.Info("Configure");

            Config.OverrideRegion.Value = overrideRegion;
            Config.Region.Value = region;

            Evaluate(overrideRegion, region);
        }

        public void Evaluate(bool overrideRegion, int region)
        {
            Logging.Info($"Evaluate ({overrideRegion}, {region})");

            if (overrideRegion)
            {
                Patches.Region = region;
                Patching.ApplyPatches();
            }
            else
            {
                Patching.RemovePatches();
            }
        }

        public string[] GetRegionTokens() => NetSys.regionNames;

        // https://doc.photonengine.com/pun/current/connection-and-authentication/regions
        // Under the "Avaliable Regions" section

        public string RegionFromToken(string regionToken) => regionToken switch
        {
            // used regions
            "us" => "USA, East",
            "usw" => "USA, West",
            "eu" => "Europe",
            // other regions
            "asia" => "Asia",
            "au" => "Australia",
            "cae" => "Canada, East",
            "hk" => "Hong Kong",
            "in" => "India",
            "jp" => "Japan",
            "za" => "South Africa",
            "sa" => "South America",
            "kr" => "South Korea",
            "tr" => "Turkey",
            "uae" => "United Arab Emirates",
            "ussc" => "USA, South Central",
            null => "Unknown",
            _ => throw new ArgumentOutOfRangeException("regionToken")
        };
    }
}
