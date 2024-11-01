using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HealthBars.Scripts.Bars.Types {
    public class CrystalSnailResourceBar : DualResourceBar {
        protected override Color PrimaryBarColor => Options.ColorHealth;
        protected override Color SecondaryBarColor => Options.ColorShield;
        protected override bool DimPrimaryWhenSecondaryVisible => true;

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

            if (!EntityUtility.TryGetComponentData<HealthCD>(entity, world, out var healthData) ||
                !EntityUtility.TryGetComponentData<EnemyActAsDestructibleCD>(entity, world,
                    out var actAsDestructibleData)) return;
            
            var num = (int) (actAsDestructibleData.healthThreshold * healthData.maxHealth);
            var num2 = healthData.maxHealth - num;
            var num3 = (float) (healthData.health - num) / num2;
            
            progress = math.clamp(num3, 0f, 1f);
            visible = progress > 0f && progress < 1f;
        }
    }
}