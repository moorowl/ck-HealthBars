using HarmonyLib;
using HealthBars.Scripts.Bars;
using System.Collections.Generic;

namespace HealthBars.Scripts.Patches {
    [HarmonyPatch]
    public static class EntityMonoBehaviour_Patch {
        internal static readonly Dictionary<int, ResourceBarHandler> BarHandlerLookup = new();

        [HarmonyPatch(typeof(EntityMonoBehaviour), nameof(EntityMonoBehaviour.UpdateDisableAnimator))]
        [HarmonyPostfix]
        public static void PostfixUpdateDisableAnimator(EntityMonoBehaviour __instance) {
            if (!__instance.entityExist)
                return;

            if (BarHandlerLookup.TryGetValue(__instance.gameObject.GetInstanceID(), out var handler))
                handler.UpdateState(__instance.entity, __instance.world, __instance);
        }
    }
}