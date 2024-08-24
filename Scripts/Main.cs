using PugMod;
using UnityEngine;

namespace HealthBars.Scripts {
    public class Main : IMod {
        public const string Version = "1.0.2";
        public const string InternalName = "HealthBars";

        internal static GameObject HealthBarPrefab { get; private set; }

        public void EarlyInit() {
            Debug.Log($"[{InternalName}]: Mod version: {Version}");
        }

        public void Init() {
            Options.Init();
        }

        public void Shutdown() { }

        public void ModObjectLoaded(Object obj) {
            if (obj.name == "HealthBar")
                HealthBarPrefab = (GameObject) obj;
        }

        public void Update() { }
    }
}
