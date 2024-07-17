using PugMod;
using System.Collections.Generic;
using UnityEngine;

namespace HealthBars.Scripts {
    public class Main : IMod {
        public const string Version = "1.0.2";
        public const string InternalName = "HealthBars";

        internal static List<PoolablePrefabBank> PoolablePrefabBanks = new();

        public void EarlyInit() {
            Debug.Log($"[{InternalName}]: Mod version: {Version}");
        }

        public void Init() {
            Options.Init();
        }

        public void Shutdown() { }

        public void ModObjectLoaded(Object obj) {
            if (obj is PoolablePrefabBank bank)
                PoolablePrefabBanks.Add(bank);
        }

        public void Update() { }
    }
}
