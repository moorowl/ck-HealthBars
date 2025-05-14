using Unity.Entities;
using UnityEngine;

namespace HealthBars.Scripts.Bars.Types {
    public class MinionDurationResourceBar : DualResourceBar {
        protected override Color PrimaryBarColor => Options.ColorDuration;
        protected override Color SecondaryBarColor => secondaryBar.color;
        
        protected override void GetPrimaryBarState(Entity entity, World world, EntityMonoBehaviour entityMono, out float progress, out bool visible) {
            progress = 0f;
            visible = false;

            if (!EntityUtility.TryGetComponentData<MinionCD>(entity, world, out var minionData))
                return;

            if (!EntityUtility.TryGetComponentData<OwnerReferenceCD>(entity, world, out var ownerData))
                return;

            if (Manager.memory.GetEntityMono(ownerData.owner) is not PlayerController { isLocal: true } || entityMono.isHidden)
                return;

            progress = minionData.normalizedLifespanTimer;
            visible = progress > 0f && progress < 1f;
        }

        protected override void GetSecondaryBarState(Entity entity, World world, EntityMonoBehaviour entityMono, out float progress, out bool visible) {
            progress = 0f;
            visible = false;
        }
    }
}