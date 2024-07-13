using PugMod;
using Unity.Mathematics;

namespace HealthBars.Scripts {
    public static class Options {
        public static float BarOpacity { get; private set; }
        public static bool AlwaysShowBar { get; private set; }

        public static void Init() {
            BarOpacity = math.clamp(RegisterAndGet("BarOpacity", "Opacity of health bars.", 0.85f), 0.1f, 1f);
            AlwaysShowBar = RegisterAndGet("AlwaysShowBar", "If set to true, shows health bars at full health.", false);
        }

        private static T RegisterAndGet<T>(string key, string description, T defaultValue = default) {
            return API.Config.Register(Main.InternalName, "Config", description, key, defaultValue).Value;
        }
    }
}
