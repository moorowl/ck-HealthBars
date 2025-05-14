using Pug.UnityExtensions;
using PugTilemap;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HealthBars.Scripts.Bars.Types {
    public class HealthAndArmorResourceBar : DualResourceBar {
        protected override Color PrimaryBarColor => Options.ColorHealth;
        protected override Color SecondaryBarColor => Options.ColorMana;

        private float _secondsInsideWall;

        private const float HideAfterSecondsInsideWall = 0.5f;
        
        protected override void GetPrimaryBarState(Entity entity, World world, EntityMonoBehaviour entityMono, out float progress, out bool visible) {
            progress = 0f;
            visible = false;

            if (!EntityUtility.TryGetComponentData<HealthCD>(entity, world, out var health))
                return;
            
            var currentHealth = entityMono.GetCurrentHealth(in health);
            var maxHealth = entityMono.GetMaxHealth();
                
            // Cocoons
            if (EntityUtility.HasComponentData<HatchWhenPlayerNearbyStateCD>(entity, world) && currentHealth == 1)
                currentHealth = 0;

            var hiddenFromBeingInsideWall = false;
            if (entityMono is WormSegment) {
                _secondsInsideWall += Time.deltaTime;
                
                var tileLookup = Manager.multiMap.GetTileLayerLookup();
                var tilePosition = gameObject.transform.parent.position.ToWorld().RoundToInt2();
                if (tileLookup.GetTopTile(tilePosition).tileType.IsWallTile())
                    _secondsInsideWall += Time.deltaTime;
                else
                    _secondsInsideWall = 0f;

                if (_secondsInsideWall > HideAfterSecondsInsideWall)
                    hiddenFromBeingInsideWall = true;
            }
                
            progress = math.clamp(currentHealth / (float) maxHealth, 0f, 1f);
            visible = !hiddenFromBeingInsideWall && progress > 0f && progress < 1f;
        }

        protected override void GetSecondaryBarState(Entity entity, World world, EntityMonoBehaviour entityMono, out float progress, out bool visible) {
            progress = 0f;
            visible = false;
        }
    }
}