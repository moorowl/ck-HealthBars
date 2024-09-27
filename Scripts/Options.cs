using PugMod;
using System.Collections.Generic;
using System.Globalization;
using Unity.Mathematics;
using UnityEngine;

namespace HealthBars.Scripts {
    public static class Options {
        private static readonly Dictionary<ObjectID, Vector3> _HealthBarOffsets = new() {
            { ObjectID.CrystalBigSnail, new Vector3(0f, 0f, -0.75f) },
            { ObjectID.SnarePlant, new Vector3(0f, 0f, -0.35f) },
            { ObjectID.SmallTentacle, new Vector3(0f, 0f, -0.275f) },
            { ObjectID.MoldTentacle, new Vector3(0f, 0f, -0.45f) },
            { ObjectID.CrystalMerchant, new Vector3(0f, 0f, 0.2f) },
            { ObjectID.BombScarab, new Vector3(0f, 0f, 0.26f) },
            { ObjectID.LavaButterfly, new Vector3(0f, 0f, 0.45f) },
            { ObjectID.Larva, new Vector3(0f, 0f, -0.3f) },
            { ObjectID.CrabEnemy, new Vector3(0f, 0f, 0.15f) },
            { ObjectID.OrbitalTurret, new Vector3(0f, 0f, 0.1f) },
            { ObjectID.OctopusTentacle, new Vector3(0.4f, 0f, 0.1f) }
        };

        private const string DefaultColor = "#db412f";

        public static float Opacity { get; private set; }
        public static bool AlwaysShow { get; private set; }
        public static Color Color { get; private set; }

        public static void Init() {
            Opacity = math.clamp(RegisterAndGet("Opacity", "Opacity of health bars.", 0.4f), 0.1f, 1f);
            AlwaysShow = RegisterAndGet("AlwaysShow", "If set to true, shows health bars at full health.", false);
            Color = ParseHexColor(RegisterAndGet("Color", "Color of health bars.", "#db412f")) ?? ParseHexColor(DefaultColor).Value;
        }

        private static T RegisterAndGet<T>(string key, string description, T defaultValue = default) {
            return API.Config.Register(Main.InternalName, "Config", description, key, defaultValue).Value;
        }

        public static Vector3 GetHealthBarOffset(ObjectID objectId) {
            return _HealthBarOffsets.GetValueOrDefault(objectId);
        }

        private static Color? ParseHexColor(string hexColor) {
            if (string.IsNullOrEmpty(hexColor) || hexColor.Length < 7)
                return null;

            hexColor = hexColor.Replace("#", "");

            return new Color(
                byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber) / 255f,
                byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber) / 255f,
                byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber) / 255f,
                1f
            );
        }
    }
}