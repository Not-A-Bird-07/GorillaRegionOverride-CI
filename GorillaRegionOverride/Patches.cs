using GorillaRegionOverride.Behaviours;
using HarmonyLib;

namespace GorillaRegionOverride
{
    [HarmonyPatch]
    public class Patches
    {
        [HarmonyPatch(typeof(NetworkSystemPUN), "get_lowestPingRegionIndex"), HarmonyPrefix]
        public static bool LowestRegionPatch(out int __result)
        {
            __result = Main.GetRegionIndex(Main.Instance.Data.region_config);
            return false;
        }

        [HarmonyPatch(typeof(NetworkSystemPUN), "GetRandomWeightedRegion"), HarmonyPrefix, HarmonyWrapSafe]
        public static bool WeightedRegionPatch(out string __result)
        {
            __result = Main.GetRegionTokens()[Main.GetRegionIndex(Main.Instance.Data.region_config)];
            return false;
        }
    }
}