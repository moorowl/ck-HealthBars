using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HealthBars.Scripts.Patches {
    [HarmonyPatch]
    public static class MemoryManager_Patch {
        internal static readonly List<GameObject> BarPrefabsToRegister = new();

        private const int InitialPoolSize = 8;
        private const int MaxPoolSize = 1024;
        private const int MaxPoolFreeSize = 1024;
        
        [HarmonyPatch(typeof(MemoryManager), nameof(MemoryManager.Init))]
        [HarmonyPrefix]
        public static void InjectPoolablePrefabs(MemoryManager __instance) {
            if (__instance.poolablePrefabBanks == null)
                return;

            var prefabBank = ScriptableObject.CreateInstance<PooledGraphicalObjectBank>();

            prefabBank.poolablePlatformScaling = new List<PoolablePrefabBank.PlatformObjectPoolScaling>();
            prefabBank.poolInitializers = BarPrefabsToRegister.Select(prefab => new PoolablePrefabBank.PoolablePrefab {
                prefab = prefab,
                initialSize = InitialPoolSize,
                maxSize = MaxPoolSize,
                maxFreeSize = MaxPoolFreeSize
            }).ToList();

            __instance.poolablePrefabBanks.Add(prefabBank);
        }
    }
}
