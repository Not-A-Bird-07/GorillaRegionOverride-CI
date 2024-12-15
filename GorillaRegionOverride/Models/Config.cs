using BepInEx.Configuration;

namespace GorillaRegionOverride.Models
{
    public class Config(ConfigFile file)
    {
        public ConfigEntry<bool> OverrideRegion = file.Bind(new ConfigDefinition(Constants.Name, "Override Region"), false, new ConfigDescription("Whether the server region is overridden"));

        public ConfigEntry<int> Region = file.Bind(new ConfigDefinition(Constants.Name, "Region"), 0, new ConfigDescription("The manual server region (overridden)"));
    }
}
