using HealthBars.Scripts.Bars;
using PugMod;
using UnityEngine;

namespace HealthBars.Scripts {
    public class Main : IMod {
        public const string Version = "1.3.4";
        public const string InternalName = "HealthBars";
        
        public void EarlyInit() {
            Debug.Log($"[{InternalName}]: Mod version: {Version}");
        }

        public void Init() {
            API.Client.OnObjectSpawnedOnClient += Utils.AssignResourceBar;
            API.Client.OnObjectDespawnedOnClient += Utils.UnassignResourceBar;
            
            Options.Init();
        }

        public void Shutdown() { }

        public void ModObjectLoaded(Object obj) {
            if (obj is GameObject gameObject && gameObject.TryGetComponent<ResourceBarBase>(out var resourceBar))
                Utils.RegisterResourceBar(resourceBar);
        }

        public void Update() { }
    }
}
