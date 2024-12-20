using GorillaComputer.Behaviours;
using GorillaComputer.Extension;
using GorillaComputer.Models;
using GorillaRegionOverride.Behaviours;
using System.Text;
using UnityEngine;

[assembly: ComputerScannable]
namespace GorillaRegionOverride.Models
{
    [ComputerCustomScreen]
    public class Screen : ComputerScreen
    {
        public override string Title => "Region";

        public override string Summary => "Use [0-9] to select region\nPress [ENTER] to toggle region override";

        public bool OverrideRegion;

        public int Region;

        public string[] RegionTokens;

        public override void OnScreenShow()
        {
            (OverrideRegion, Region) = Singleton<Main>.Instance.Data();
            RegionTokens = Singleton<Main>.Instance.GetRegionTokens();
        }

        public override string GetContent()
        {
            StringBuilder str = new();

            str.AppendLine($"Override Region: {OverrideRegion}").AppendLine();

            if (OverrideRegion)
            {
                str.AppendLine($"Region: {Singleton<Main>.Instance.RegionFromToken(RegionTokens[Region])}");
            }

            return str.ToString();
        }

        public override void ProcessScreen(KeyBinding key)
        {
            bool dirty = false;

            if (key == KeyBinding.enter)
            {
                OverrideRegion ^= true;
                dirty = true;
            }

            if (key.TryParseNumber(out int number))
            {
                Region = Mathf.Clamp(number - 1, 0, RegionTokens.Length - 1);
                dirty = true;
            }

            if (!dirty) return;

            UpdateScreen();
            Singleton<Main>.Instance.Configure(OverrideRegion, Region);
        }
    }
}
