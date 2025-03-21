using GorillaRegionOverride.Models;
using GorillaRegionOverride.Tools;
using HarmonyLib;
using System;
using System.Linq;

namespace GorillaRegionOverride.Behaviours
{
    internal class Main : Singleton<Main>
    {
        private static NetworkSystem NetSys => NetworkSystem.Instance;

        public Patching Patching = new(Constants.Guid);

        public Config Config = new(Plugin.TiedConfigFile);

        protected override void Initialize()
        {
            Logging.Info("Initialize");

            if (NetSys && !NetSys.GetComponent<NetworkSystemPUN>())
            {
                Logging.Warn("NetSys is not based on PUN backend - this mod won't have any effect on server region");
            }

            (ERegionConfig region_config, int region_index) = Data;
            Evaluate(region_config, region_index);
        }

        public (ERegionConfig region_config, int region_index) Data => (Config.OverrideRegion.Value, Config.Region.Value);

        public void Configure(ERegionConfig region_config, int region_index)
        {
            Logging.Info("Configure");

            Config.OverrideRegion.Value = region_config;
            Config.Region.Value = region_index;

            Evaluate(region_config, region_index);
        }

        public void Evaluate(ERegionConfig region_config, int region_index)
        {
            Logging.Info($"Evaluate ({region_config}, {region_index})");

            if (region_config != ERegionConfig.AutoLowPing) // AutoLowPing is how the game handles it
            {
                Patching.ApplyPatches();
            }
            else
            {
                Patching.RemovePatches();
            }
        }

        public static string[] GetRegionTokens() => NetSys.regionNames;

        // Under the "Avaliable Regions" section: https://doc.photonengine.com/pun/current/connection-and-authentication/regions
        public static string RegionFromToken(string regionToken) => regionToken switch
        {
            // used regions (by game)
            "us" => "USA, East",
            "usw" => "USA, West",
            "eu" => "Europe",
            // other regions (might be used by another mod for all i know)
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
            _ => throw new ArgumentOutOfRangeException("regionToken")
        };

        public static string GetConfigName(ERegionConfig region_config) => region_config switch
        {
            ERegionConfig.AutoLowPing => "Auto (connection)",
            ERegionConfig.AutoHighPlayers => "Auto (population)",
            ERegionConfig.ManualRegion => "Manual",
            _ => throw new ArgumentOutOfRangeException("region_config")
        };

        public static NetworkRegionInfo GetRegionInfo(int region_index) 
        {
            NetworkRegionInfo[] regionData = (NetworkRegionInfo[])AccessTools.Field(typeof(NetworkSystemPUN), "regionData").GetValue(NetSys);
            return regionData[region_index];
        }

        public static int GetRegionIndex(ERegionConfig region_config)
        {
            if (region_config == ERegionConfig.AutoLowPing)
            {
                NetworkRegionInfo[] regionData = (NetworkRegionInfo[])AccessTools.Field(typeof(NetworkSystemPUN), "regionData").GetValue(NetSys);
                int min = regionData.Min(data => data.pingToRegion);
                int index = Array.IndexOf(regionData, Array.Find(regionData, data => data.pingToRegion == min));
                return index;
            }
            if (region_config == ERegionConfig.AutoHighPlayers)
            {
                NetworkRegionInfo[] regionData = (NetworkRegionInfo[])AccessTools.Field(typeof(NetworkSystemPUN), "regionData").GetValue(NetSys);
                int max = regionData.Max(data => data.playersInRegion);
                int index = Array.IndexOf(regionData, Array.Find(regionData, data => data.playersInRegion == max));
                return index;
            }
            return Instance.Data.region_index;
        }
    }
}
