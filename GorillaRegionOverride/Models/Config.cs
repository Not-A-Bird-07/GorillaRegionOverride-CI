using BepInEx.Configuration;

namespace GorillaRegionOverride.Models
{
    public class Config(ConfigFile file)
    {
        public ConfigEntry<ERegionConfig> OverrideRegion = file.Bind(new ConfigDefinition(Constants.Name, "Region Config"), ERegionConfig.AutoLowPing, new ConfigDescription("How designated server ping is selected"));

        public ConfigEntry<int> Region = file.Bind(new ConfigDefinition(Constants.Name, "Region"), 0, new ConfigDescription("The manual server region (overridden)"));
    }
}
