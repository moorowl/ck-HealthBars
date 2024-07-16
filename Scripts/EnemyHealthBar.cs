using Unity.Mathematics;
using UnityEngine;

namespace HealthBars.Scripts {
    public class EnemyHealthBar : HealthBar, IPoolable {
        public static bool HasHealthBar(EntityMonoBehaviour entity) {
            return (entity.objectInfo.objectType == ObjectType.Creature || (entity.objectInfo.objectType == ObjectType.PlayerType && entity != Manager.main.player)) && entity.GetMaxHealth() > 1;
        }

        public float Opacity {
            get => background.color.a;
            set {
                background.color = background.color.ColorWithNewAlpha(value);
                bar.color = bar.color.ColorWithNewAlpha(value);
            }
        }
        public float Progress {
            get => healthBarMaskPivot.transform.localScale.x;
            set {
                healthBarMaskPivot.transform.localScale = new Vector3(value, 1f, 1f);
            }
        }

        private EntityMonoBehaviour _lastAssignedEntity;

        public void UpdateState(EntityMonoBehaviour entity) {
            if (entity == null)
                return;
            if (_lastAssignedEntity != entity)
                Opacity = 0f;

            var healthRatio = GetNormalizedHealth(entity);
            var oldHealthRatio = Progress;

            UpdateProgress(healthRatio, oldHealthRatio, _lastAssignedEntity == entity);
            UpdateOpacity(healthRatio, oldHealthRatio);

            _lastAssignedEntity = entity;
            root.SetActive(Opacity > 0f);
        }

        private void UpdateProgress(float healthRatio, float oldHealthRatio, bool allowLerp) {
            const float LargeHitThreshold = 0.1f;
            const float LargeHitLerpSpeed = 15f;
            const float HealingLerpSpeed = 5f;

            var lerpSpeed = 0f;
            if (allowLerp) {
                if (healthRatio > oldHealthRatio)
                    lerpSpeed = HealingLerpSpeed;
                if (oldHealthRatio - healthRatio >= LargeHitThreshold)
                    lerpSpeed = LargeHitLerpSpeed;
            }

            Progress = lerpSpeed > 0f ? math.lerp(oldHealthRatio, healthRatio, lerpSpeed * Time.deltaTime) : healthRatio;
            if (math.abs(healthRatio - Progress) < 0.01f)
                Progress = healthRatio;
        }

        private void UpdateOpacity(float healthRatio, float oldHealthRatio) {
            var targetOpacity = 0f;
            if ((healthRatio > 0f && healthRatio < 1f) || (Options.AlwaysShowBar && healthRatio >= 1f))
                targetOpacity = Options.BarOpacity;

            var newOpacity = 0f;
            if (!Manager.prefs.hideInGameUI && !Manager.main.player.guestMode) {
                newOpacity = math.lerp(Opacity, targetOpacity, 25f * Time.deltaTime);

                if (math.abs(targetOpacity - newOpacity) < 0.05f)
                    newOpacity = targetOpacity;
            }

            Opacity = newOpacity;
        }

        private static float GetNormalizedHealth(EntityMonoBehaviour entity) {
            var healthData = EntityUtility.GetComponentData<HealthCD>(entity.entity, entity.world);
            var conditionEffectValues = EntityUtility.GetConditionEffectValues(entity.entity, entity.world);

            var currentHealth = healthData.health;
            var maxHealth = healthData.GetMaxHealthWithConditions(entity.entity, conditionEffectValues);

            // Cocoons
            if (EntityUtility.HasComponentData<HatchWhenPlayerNearbyStateCD>(entity.entity, entity.world) && currentHealth == 1)
                return 0f;

            return math.clamp(1f - (float) (maxHealth - currentHealth) / maxHealth, 0f, 1f);
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
