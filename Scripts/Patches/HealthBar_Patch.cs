using HarmonyLib;

namespace HealthBars.Scripts.Patches {
    [HarmonyPatch]
    public static class HealthBar_Patch {
        [HarmonyPatch(typeof(HealthBar), nameof(HealthBar.UpdateHealthBar))]
        [HarmonyPrefix]
        public static bool PrefixUpdateHealthBar(HealthBar __instance, float value) {
            if (__instance is not EnemyHealthBar healthBar)
                return true;

            healthBar.UpdateState(__instance.GetComponentInParent<EntityMonoBehaviour>());

            return false;
        }
    }
}
