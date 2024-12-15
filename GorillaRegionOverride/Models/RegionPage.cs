using GorillaComputer;
using GorillaComputer.Extension;
using GorillaComputer.Model;
using GorillaNetworking;
using GorillaRegionOverride.Behaviours;
using System.Text;
using UnityEngine;

[assembly: AutoRegister]
namespace GorillaRegionOverride.Models
{
    [AutoRegister]
    public class RegionPage : ComputerFunction
    {
        public override string Name => "Region";

        public override string Description => "Configure your region settings to use automatically choose a region or select your own";

        public bool OverrideRegion;

        public int Region;

        public string[] RegionTokens;

        public override void OnFunctionOpened()
        {
            (OverrideRegion, Region) = Singleton<Main>.Instance.Data();
            RegionTokens = Singleton<Main>.Instance.GetRegionTokens();
        }

        public override string GetFunctionText()
        {
            StringBuilder str = new();

            str.AppendLine($"Override Region: {OverrideRegion}").AppendLine();

            if (OverrideRegion)
            {
                str.AppendLine($"Region: {Singleton<Main>.Instance.RegionFromToken(RegionTokens[Region])}");
            }

            return str.ToString();
        }

        public override void OnKeyPressed(GorillaKeyboardBindings key)
        {
            bool dirty = false;

            if (key == GorillaKeyboardBindings.enter)
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

            UpdateMonitor();
            Singleton<Main>.Instance.Configure(OverrideRegion, Region);
        }
    }
}
