using HarmonyLib;

namespace GorillaRegionOverride.Models
{
    public class Patching(string id)
    {
        public Harmony Instance = new(id);

        public bool IsPatched;

        public void ApplyPatches()
        {
            if (Instance == null || IsPatched) return;
            IsPatched = true;
            Instance.PatchAll(typeof(Plugin).Assembly);
        }

        public void RemovePatches()
        {
            if (Instance == null || !IsPatched) return;
            IsPatched = false;
            Instance.UnpatchSelf();
        }
    }
}
