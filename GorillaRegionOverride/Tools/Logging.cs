using BepInEx.Logging;

namespace GorillaRegionOverride.Tools
{
    public static class Logging
    {
        public static void Info(object message) => LogMessage(LogLevel.Info, message);

        public static void Warn(object message) => LogMessage(LogLevel.Warning, message);

        public static void Error(object message) => LogMessage(LogLevel.Error, message);

        public static void LogMessage(LogLevel level, object message)
        {
            bool debug = false;
#if DEBUG
            debug = true;
#endif
            if (!Constants.DebugLogExclusive || debug)
            {
                Plugin.TiedLogSource.Log(level, message);
            }
        }
    }
}
