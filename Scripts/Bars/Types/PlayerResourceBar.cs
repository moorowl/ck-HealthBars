using PlayerState;
using Unity.Entities;
using UnityEngine;

namespace HealthBars.Scripts.Bars.Types {
    public class PlayerResourceBar : DualResourceBar {
        protected override Color PrimaryBarColor => Options.ColorHealth;
        protected override Color SecondaryBarColor => Options.ColorMana;
        
        protected override void GetPrimaryBarState(Entity entity, World world, EntityMonoBehaviour entityMono, out float progress, out bool visible) {
            progress = 0f;
            visible = false;
            
            if (!Options.ShowPlayerHealth || entityMono is not PlayerController playerController || !playerController.XScaler.gameObject.activeSelf || (!Options.ShowOnLocalPlayer && playerController.isLocal))
                return;

            if (EntityUtility.TryGetComponentData<PlayerStateCD>(entity, world, out var playerState) && PlayerController.IsDyingOrDead(playerState))
                return;

            if (!Utils.TryGetNormalizedHealth(entity, world, entityMono, out var health))
                return;
            
            progress = health;
            visible = progress > 0f && progress < 1f;
        }

        protected override void GetSecondaryBarState(Entity entity, World world, EntityMonoBehaviour entityMono, out float progress, out bool visible) {
            progress = 0f;
            visible = false;

            if (!Options.ShowPlayerMana || entityMono is not PlayerController playerController || !playerController.XScaler.gameObject.activeSelf || (!Options.ShowOnLocalPlayer && playerController.isLocal))
                return;

            if (EntityUtility.TryGetComponentData<PlayerStateCD>(entity, world, out var playerState) && PlayerController.IsDyingOrDead(playerState))
                return;

            if (!Utils.TryGetNormalizedMana(entity, world, playerController, out var mana))
                return;

            progress = mana;
            visible = PugDatabase.HasComponent<ConsumesManaCD>(playerController.GetEquippedSlot().objectData.objectID) && progress < 1f;
        }
    }
}