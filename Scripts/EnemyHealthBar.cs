using UnityEngine;
using Unity.Mathematics;

namespace HealthBars.Scripts {
    public class EnemyHealthBar : HealthBar, IPoolable {
        public static bool HasHealthBar(EntityMonoBehaviour entity) {
            return entity.entityExist
                && (entity.objectInfo.objectType == ObjectType.Creature || (entity.objectInfo.objectType == ObjectType.PlayerType && entity != Manager.main.player))
                && entity is not WormSegment
                && entity.GetMaxHealth() > 1;
        }

        private EntityMonoBehaviour _lastAssignedEntity;

        public float opacityLerpSpeed;
        public float largeHitThreshold;
        public float largeHitLerpSpeed;
        public float healingLerpSpeed;

        public float Opacity {
            get => background.color.a;
            set {
                background.color = background.color.ColorWithNewAlpha(value);
                bar.color = Options.Color.ColorWithNewAlpha(value);
            }
        }
        public float Progress {
            get => healthBarMaskPivot.transform.localScale.x;
            set {
                healthBarMaskPivot.transform.localScale = new Vector3(value, 1f, 1f);
            }
        }

        public void UpdateState() {
            var assignedEntity = gameObject.GetComponentInParent<EntityMonoBehaviour>();
            if (assignedEntity == null || !assignedEntity.entityExist)
                return;

            var healthRatio = GetNormalizedHealth(assignedEntity);

            // Update opacity
            if (_lastAssignedEntity != assignedEntity)
                Opacity = 0f;

            var targetOpacity = 0f;
            if ((healthRatio > 0f && healthRatio < 1f) || (Options.AlwaysShow && healthRatio >= 1f))
                targetOpacity = Options.Opacity;

            var newOpacity = 0f;
            if (!Manager.prefs.hideInGameUI && !Manager.main.player.guestMode) {
                newOpacity = math.lerp(Opacity, targetOpacity, opacityLerpSpeed * Time.deltaTime);

                if (math.abs(targetOpacity - newOpacity) < 0.05f)
                    newOpacity = targetOpacity;
            }

            root.SetActive(newOpacity > 0f);
            Opacity = newOpacity;

            // Update progress
            Progress = UpdateProgress(Progress, healthRatio, _lastAssignedEntity == assignedEntity);

            _lastAssignedEntity = assignedEntity;
        }

        private float UpdateProgress(float oldRatio, float newRatio, bool allowLerp) {
            var lerpSpeed = 0f;
            if (allowLerp) {
                if (newRatio > oldRatio)
                    lerpSpeed = healingLerpSpeed;
                if (oldRatio - newRatio >= largeHitThreshold)
                    lerpSpeed = largeHitLerpSpeed;
            }

            var progress = lerpSpeed > 0f ? math.lerp(oldRatio, newRatio, lerpSpeed * Time.deltaTime) : newRatio;
            if (math.abs(newRatio - progress) < 0.01f)
                progress = newRatio;

            return progress;
        }

        private static float GetNormalizedHealth(EntityMonoBehaviour entity) {
            var healthData = EntityUtility.GetComponentData<HealthCD>(entity.entity, entity.world);
            var conditionEffectValues = EntityUtility.GetConditionEffectValues(entity.entity, entity.world);

            var currentHealth = healthData.health;
            var maxHealth = healthData.GetMaxHealthWithConditions(conditionEffectValues);

            // Cocoons
            if (EntityUtility.HasComponentData<HatchWhenPlayerNearbyStateCD>(entity.entity, entity.world) && currentHealth == 1)
                return 0f;

            return math.clamp(1f - (float)(maxHealth - currentHealth) / maxHealth, 0f, 1f);
        }

        private void OnValidate() {
            var barSize = new Vector2(barWidth * 0.0625f, 0.125f);
            if (autoSizeBar && bar != null && bar.size != barSize) {
                bar.size = barSize;
                background.size = new Vector2(barWidth * 0.0625f + 0.125f, 0.25f);
                healthBarMaskPivot.transform.localPosition = new Vector3((0f - barWidth) * 0.0625f * 0.5f, 0f, 0f);
                bar.transform.localPosition = new Vector3(barWidth * 0.0625f * 0.5f, 0f, 0f);
            }
        }

        #region Pooling stuff
        private IPoolSystem _pool;

        public bool isPooled => _pool != null;
        public bool isFree => _pool.IsFree(gameObject);

        public void OnAllocation(IPoolSystem pool) {
            _pool = pool;
        }

        public virtual void OnOccupied() { }

        public virtual void OnFree() { }

        public virtual void Free() {
            if (isPooled) {
                _pool.Free(gameObject);
                return;
            }

            OnFree();
            gameObject.Destroy_Clean();
        }

        public virtual void OnDestroy() { }
        #endregion
    }
}
