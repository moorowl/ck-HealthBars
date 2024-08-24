using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace HealthBars.Scripts.Patches {
    [HarmonyPatch]
    public static class MemoryManager_Patch {
        [HarmonyPatch(typeof(MemoryManager), nameof(MemoryManager.Init))]
        [HarmonyPrefix]
        public static void InjectPoolablePrefabs(MemoryManager __instance) {
            if (__instance.poolablePrefabBanks == null)
                return;

            Debug.Log($"HealthBarPrefab: {Main.HealthBarPrefab}");
            var prefabBank = ScriptableObject.CreateInstance<PooledGraphicalObjectBank>();

            prefabBank.poolablePlatformScaling = new List<PoolablePrefabBank.PlatformObjectPoolScaling>();
            prefabBank.poolInitializers = new List<PoolablePrefabBank.PoolablePrefab> {
                new PoolablePrefabBank.PoolablePrefab {
                    prefab = Main.HealthBarPrefab,
                    initialSize = 8,
                    maxSize = 1024,
                    maxFreeSize = 1024,
                }
            };

            __instance.poolablePrefabBanks.Add(prefabBank);
        }
    }
}
