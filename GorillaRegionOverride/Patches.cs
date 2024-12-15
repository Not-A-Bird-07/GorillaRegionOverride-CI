using GorillaRegionOverride.Behaviours;
using HarmonyLib;

namespace GorillaRegionOverride
{
    [HarmonyPatch]
    public class Patches
    {
        /// <summary>
        /// The server region index to use, this should be set through Main.Evaluate to align with the configuration as well
        /// </summary>
        public static int Region;

        [HarmonyPatch(typeof(NetworkSystemPUN), "get_lowestPingRegionIndex"), HarmonyPrefix]
        public static bool LowestRegionPatch(out int __result)
        {
            __result = Region;
            return false;
        }

        [HarmonyPatch(typeof(NetworkSystemPUN), "GetRandomWeightedRegion"), HarmonyPrefix, HarmonyWrapSafe]
        public static bool WeightedRegionPatch(out string __result)
        {
            __result = Singleton<Main>.Instance.GetRegionTokens()[Region];
            return false;
        }
    }
}