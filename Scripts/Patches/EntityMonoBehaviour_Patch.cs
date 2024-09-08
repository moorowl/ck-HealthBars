using HarmonyLib;
using UnityEngine;

namespace HealthBars.Scripts.Patches {
    [HarmonyPatch]
    public static class EntityMonoBehaviour_Patch {
        [HarmonyPatch(typeof(EntityMonoBehaviour), "UpdateHealthBar")]
        [HarmonyPrefix]
        public static void PrefixUpdateHealthBar(EntityMonoBehaviour __instance) {
            if (__instance.optionalHealthBar == null && EnemyHealthBar.HasHealthBar(__instance)) {
                var healthBar = Manager.memory.GetFreeComponent<EnemyHealthBar>(true, true);
                if (healthBar == null)
                    return;

                healthBar.transform.SetParent(__instance.transform, false);
                healthBar.transform.localPosition = Options.GetHealthBarOffset(__instance.objectData.objectID);
                healthBar.transform.localScale = Vector3.one;
                healthBar.transform.localRotation = Quaternion.identity;
                healthBar.OnOccupied();

                __instance.optionalHealthBar = healthBar;
            }
        }

        [HarmonyPatch(typeof(EntityMonoBehaviour), "OnFree")]
        [HarmonyPostfix]
        public static void PostfixOnFree(EntityMonoBehaviour __instance) {
            if (__instance.optionalHealthBar is EnemyHealthBar healthBar && healthBar.isPooled && !healthBar.isFree) {
                healthBar.Free();
                __instance.optionalHealthBar = null;
            }
        }
    }
}