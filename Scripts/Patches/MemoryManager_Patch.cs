using HarmonyLib;

namespace HealthBars.Scripts.Patches {
    [HarmonyPatch]
    public static class MemoryManager_Patch {
        [HarmonyPatch(typeof(MemoryManager), nameof(MemoryManager.Init))]
        [HarmonyPrefix]
        public static void InjectPoolablePrefabs(MemoryManager __instance) {
            if (__instance.poolablePrefabBanks == null)
                return;

            __instance.poolablePrefabBanks.AddRange(Main.PoolablePrefabBanks);
        }
    }
}
