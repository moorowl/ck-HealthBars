using System;
using PugMod;
using System.Globalization;
using HealthBars.Scripts.Bars;
using Unity.Mathematics;
using UnityEngine;

namespace HealthBars.Scripts {
    public static class Options {
        private const string DefaultHealthColor = "#ff3d3d";
        private const string DefaultShieldColor = "#008eff";
        private const string DefaultManaColor = DefaultShieldColor;
        private const string DefaultDurationColor = DefaultShieldColor;
        private const string DefaultProgressBarColor = DefaultShieldColor;

        public static float Opacity { get; private set; }

        public static bool ShowHealth { get; private set; }
        public static bool ShowPlayerHealth { get; private set; }
        public static bool ShowPlayerMana { get; private set; }
        public static bool ShowMinionDuration { get; private set; }
        public static bool ShowOreBoulderProgress { get; private set; }
        public static bool ShowOnLocalPlayer { get; private set; }
        
        public static Color ColorHealth { get; private set; }
        public static Color ColorShield { get; private set; }
        public static Color ColorMana { get; private set; }
        public static Color ColorDuration { get; private set; }
        public static Color ColorProgressBar { get; private set; }

        public static void Init() {
            // General section
            var version = RegisterAndGet("General", "Version", "Config version.", 0);
            
            Opacity = RegisterAndGet("General", "Opacity", "The opacity of health bars.", 0.4f, value => {
                return math.clamp(value, 0.1f, 1f);
            });
            
            ShowHealth = RegisterAndGet("General", "ShowHealth", "Enables health bars on enemies/cattle/merchants/some objects.", true);
            ShowPlayerHealth = RegisterAndGet("General", "ShowPlayerHealth", "Enables health bars on players.", true);
            ShowPlayerMana = RegisterAndGet("General", "ShowPlayerMana", "Enables mana bars on players. ShowPlayerHealth must also be enabled.", true);
            ShowOnLocalPlayer = RegisterAndGet("General", "ShowOnLocalPlayer", "Enables health and mana bars on the local player (if their respective options are also enabled).", false);
            ShowMinionDuration = RegisterAndGet("General", "ShowMinionDuration", "Enables duration bars for minions.", false);
            // ShowOreBoulderProgress = RegisterAndGet("General", "ShowOreBoulderProgress", "Enables progress bars for ore boulders.", true);
            
            // Colors section
            ColorHealth = RegisterAndGet("Colors", "Health", "Fill color of health bars.", DefaultHealthColor, value => {
                return Utils.ParseHexColor(value) ?? Utils.ParseHexColor(DefaultHealthColor).Value;
            });
            ColorShield = RegisterAndGet("Colors", "Shield", "Fill color of shield bars (Crystal Snail).", DefaultShieldColor, value => {
                return Utils.ParseHexColor(value) ?? Utils.ParseHexColor(DefaultShieldColor).Value;
            });
            ColorMana = RegisterAndGet("Colors", "Mana", "Fill color of a player's mana bar.", DefaultManaColor, value => {
                return Utils.ParseHexColor(value) ?? Utils.ParseHexColor(DefaultManaColor).Value;
            });
            ColorDuration = RegisterAndGet("Colors", "Duration", "Fill color of minion duration bars.", DefaultDurationColor, value => {
                return Utils.ParseHexColor(value) ?? Utils.ParseHexColor(DefaultDurationColor).Value;
            });
            /*ColorProgressBar = RegisterAndGet("Colors", "ProgressBar", "Fill color of progress bars (ore boulders).", DefaultProgressBarColor, value => {
                return Utils.ParseHexColor(value) ?? Utils.ParseHexColor(DefaultProgressBarColor).Value;
            });*/
        }

        private static T RegisterAndGet<T>(string section, string key, string description, T defaultValue) {
            return API.Config.Register(Main.InternalName, section, description, key, defaultValue).Value;
        }
        
        private static TValue RegisterAndGet<T, TValue>(string section, string key, string description, T defaultValue, Func<T, TValue> parser) {
            return parser(RegisterAndGet(section, key, description, defaultValue));
        }

        public static bool ShouldShowResourceBar(ResourceBarType type) {
            return type switch {
                ResourceBarType.HealthAndArmor => ShowHealth,
                ResourceBarType.Player => ShowPlayerHealth,
                ResourceBarType.CrystalSnail => ShowHealth,
                ResourceBarType.OreBoulder => ShowOreBoulderProgress,
                ResourceBarType.MinionDuration => ShowMinionDuration,
                _ => throw new NotImplementedException()
            };
        }
    }
}