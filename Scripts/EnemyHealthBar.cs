using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace HealthBars.Scripts {
    public class EnemyHealthBar : HealthBar, IPoolable {
        private static readonly Dictionary<ObjectID, Vector3> _HealthBarOffsets = new() {
            { ObjectID.CrystalBigSnail, new Vector3(0f, 0f, -0.75f) },
            { ObjectID.SnarePlant, new Vector3(0f, 0f, -0.35f) },
            { ObjectID.SmallTentacle, new Vector3(0f, 0f, -0.25f) },
            { ObjectID.SmallTentacle, new Vector3(0f, 0f, -0.275f) },
            { ObjectID.CrystalMerchant, new Vector3(0f, 0f, 0.2f) },
            { ObjectID.BombScarab, new Vector3(0f, 0f, 0.26f) },
            { ObjectID.LavaButterfly, new Vector3(0f, 0f, 0.45f) },
            { ObjectID.Larva, new Vector3(0f, 0f, -0.3f) },
            { ObjectID.CrabEnemy, new Vector3(0f, 0f, 0.15f) },
            { ObjectID.OrbitalTurret, new Vector3(0f, 0f, 0.1f) }
        };

        public static bool ShouldShowHealthBar(EntityMonoBehaviour entity) {
            return entity.objectData.objectID != ObjectID.WormSegment
                && entity.objectData.objectID != ObjectID.ClayWormSegment
                && (entity.objectInfo.objectType == ObjectType.Creature || (entity.objectInfo.objectType == ObjectType.PlayerType && entity != Manager.main.player))
                && entity.GetMaxHealth() > 1;
        }

        public static Vector3 GetHealthBarOffset(EntityMonoBehaviour entity) {
            if (_HealthBarOffsets.TryGetValue(entity.objectData.objectID, out var offset))
                return offset;

            return Vector3.zero;
        }

        public Color staticColor;

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

        public void UpdateVisuals(float healthRatio) {
            healthRatio = math.clamp(healthRatio, 0f, 1f);

            UpdateAlpha(healthRatio);
            UpdateProgress(healthRatio);
        }

        public Color GetFillColor() {
            return staticColor;
        }

        private void UpdateAlpha(float healthRatio) {
            var currentAlpha = background.color.a;
            var targetAlpha = 0f;

            if ((healthRatio > 0f && healthRatio < 1f) || (Options.AlwaysShowBar && healthRatio >= 1f))
                targetAlpha = Options.BarOpacity;

            var newAlpha = 0f;
            if (!Manager.prefs.hideInGameUI) {
                newAlpha = math.lerp(currentAlpha, targetAlpha, 25f * Time.deltaTime);

                if (math.abs(targetAlpha - newAlpha) < 0.05f)
                    newAlpha = targetAlpha;
            }
 
            background.color = background.color.ColorWithNewAlpha(newAlpha);
            bar.color = bar.color.ColorWithNewAlpha(newAlpha);

            root.SetActive(newAlpha > 0f);
        }

        private void UpdateProgress(float healthRatio) {
            const float LargeHitThreshold = 0.1f;
            const float LargeHitLerpSpeed = 15f;
            const float HealingLerpSpeed = 5f;

            var oldHealthRatio = healthBarMaskPivot.transform.localScale.x;

            var lerpSpeed = 0f;
            if (healthRatio > oldHealthRatio)
                lerpSpeed = HealingLerpSpeed;
            if (oldHealthRatio - healthRatio >= LargeHitThreshold)
                lerpSpeed = LargeHitLerpSpeed;

            var progress = lerpSpeed > 0f ? math.lerp(oldHealthRatio, healthRatio, lerpSpeed * Time.deltaTime) : healthRatio;
            if (math.abs(healthRatio - progress) < 0.01f)
                progress = healthRatio;

            healthBarMaskPivot.transform.localScale = new Vector3(progress, 1f, 1f);
            bar.color = GetFillColor().ColorWithNewAlpha(bar.color.a);
        }
    }
}
