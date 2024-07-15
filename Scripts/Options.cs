using PugMod;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace HealthBars.Scripts {
    public static class Options {
        private static readonly Dictionary<ObjectID, Vector3> _HealthBarOffsets = new() {
            { ObjectID.CrystalBigSnail, new Vector3(0f, 0f, -0.75f) },
            { ObjectID.SnarePlant, new Vector3(0f, 0f, -0.35f) },
            { ObjectID.SmallTentacle, new Vector3(0f, 0f, -0.275f) },
            { ObjectID.CrystalMerchant, new Vector3(0f, 0f, 0.2f) },
            { ObjectID.BombScarab, new Vector3(0f, 0f, 0.26f) },
            { ObjectID.LavaButterfly, new Vector3(0f, 0f, 0.45f) },
            { ObjectID.Larva, new Vector3(0f, 0f, -0.3f) },
            { ObjectID.CrabEnemy, new Vector3(0f, 0f, 0.15f) },
            { ObjectID.OrbitalTurret, new Vector3(0f, 0f, 0.1f) },
            { ObjectID.ClayWormSegment, new Vector3(0f, 0.5f, 0.725f) },
            { ObjectID.WormSegment, new Vector3(0f, 0.5f, 0.7f) }
        };

        public static float BarOpacity { get; private set; }
        public static bool AlwaysShowBar { get; private set; }

        public static void Init() {
            BarOpacity = math.clamp(RegisterAndGet("BarOpacity", "Opacity of health bars.", 0.85f), 0.1f, 1f);
            AlwaysShowBar = RegisterAndGet("AlwaysShowBar", "If set to true, shows health bars at full health.", false);
        }

        private static T RegisterAndGet<T>(string key, string description, T defaultValue = default) {
            return API.Config.Register(Main.InternalName, "Config", description, key, defaultValue).Value;
        }

        public static Vector3 GetHealthBarOffset(ObjectID objectId) {
            return _HealthBarOffsets.GetValueOrDefault(objectId);
        }
    }
}
