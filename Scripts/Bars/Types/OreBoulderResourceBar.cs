using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HealthBars.Scripts.Bars.Types {
    public class OreBoulderResourceBar : DualResourceBar {
        protected override Color PrimaryBarColor => Options.ColorHealth;
        protected override Color SecondaryBarColor => Options.ColorProgressBar;
        
        protected override void GetPrimaryBarState(Entity entity, World world, EntityMonoBehaviour entityMono, out float progress, out bool visible) {
            progress = 0f;
            visible = false;

            if (!Utils.TryGetNormalizedHealth(entity, world, entityMono, out var health))
                return;
            
            progress = health;
            visible = progress > 0f && progress < 1f;   
        }

        protected override void GetSecondaryBarState(Entity entity, World world, EntityMonoBehaviour entityMono, out float progress, out bool visible) {
            progress = 0f;
            visible = false;

            if (EntityUtility.TryGetComponentData<HealthCD>(entity, world, out var health) && EntityUtility.TryGetComponentData<DropsLootWhenDamagedCD>(entity, world,
                    out var dropsLootWhenDamaged)) {
                var damageToDeal = (float) dropsLootWhenDamaged.damageToDealToDropLoot;

                progress = math.clamp(1f - (health.health % damageToDeal / damageToDeal), 0f, 1f);
                visible = health.health > 0 && health.health < health.maxHealth;
            }
        }
    }
}