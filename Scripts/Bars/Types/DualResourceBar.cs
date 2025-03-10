using NaughtyAttributes;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HealthBars.Scripts.Bars.Types {
    public abstract class DualResourceBar : ResourceBarBase {
        private static readonly Color ImmuneColor = new(0.55f, 0.55f, 0.55f, 1f);
        
        public GameObject primaryBarRoot;
        public GameObject primaryBarMaskPivot;
        public SpriteRenderer primaryBar;
        public SpriteRenderer primaryBarBackground;
        public GameObject secondaryBarRoot;
        public GameObject secondaryBarMaskPivot;
        public SpriteRenderer secondaryBar;
        public SpriteRenderer secondaryBarBackground;
        public int barWidth = 20;

        public bool lerpLargeHits = true;
        [ShowIf("lerpLargeHits")]
        public float largeHitThreshold = 0.1f;
        [ShowIf("lerpLargeHits")]
        public float largeHitLerpSpeed = 15f;

        public bool lerpHealing = true;
        [ShowIf("lerpHealing")]
        public float healingLerpSpeed = 5f;
        public float opacityLerpSpeed = 25f;

        public float PrimaryBarProgress {
            get => primaryBarMaskPivot.transform.localScale.x;
            set => primaryBarMaskPivot.transform.localScale = new Vector3(value, 1f, 1f);
        }
        public float SecondaryBarProgress {
            get => secondaryBarMaskPivot.transform.localScale.x;
            set => secondaryBarMaskPivot.transform.localScale = new Vector3(value, 1f, 1f);
        }

        public float PrimaryBarOpacity {
            get => primaryBar.color.a;
            set {
                primaryBar.color = (_secondaryIsVisible && DimPrimaryWhenSecondaryVisible ? ImmuneColor : PrimaryBarColor).ColorWithNewAlpha(value);
                primaryBarBackground.color = primaryBarBackground.color.ColorWithNewAlpha(value);
                primaryBarRoot.SetActive(value > 0f);
            }
        }
        public float SecondaryBarOpacity {
            get => secondaryBar.color.a;
            set {
                secondaryBar.color = SecondaryBarColor.ColorWithNewAlpha(value);
                secondaryBarBackground.color = secondaryBarBackground.color.ColorWithNewAlpha(value);
                secondaryBarRoot.SetActive(value > 0f);
            }
        }
        
        protected abstract Color PrimaryBarColor { get; }
        protected abstract Color SecondaryBarColor { get; }

        protected virtual bool DimPrimaryWhenSecondaryVisible { get; } = false;

        private bool _secondaryIsVisible;
        private bool _wasJustAssigned;

        public override void OnOccupied() {
            PrimaryBarOpacity = 0f;
            SecondaryBarOpacity = 0f;

            _wasJustAssigned = true;
        }

        public override void UpdateState(Entity entity, World world, EntityMonoBehaviour entityMono) {
            GetPrimaryBarState(entity, world, entityMono, out var primaryProgress, out var primaryVisible);
            GetSecondaryBarState(entity, world, entityMono, out var secondaryProgress, out var secondaryVisible);

            _secondaryIsVisible = secondaryVisible && secondaryProgress > 0f;
            
            // Update progress
            primaryProgress = UpdateProgress(PrimaryBarProgress, primaryProgress, !_wasJustAssigned);
            secondaryProgress = UpdateProgress(SecondaryBarProgress, secondaryProgress, !_wasJustAssigned);
            
            // Update opacity
            // Primary is always shown if secondary is
            var primaryTargetOpacity = (primaryVisible || secondaryVisible) ? Options.Opacity : 0f;
            var secondaryTargetOpacity = secondaryVisible ? Options.Opacity : 0f;
            primaryTargetOpacity = math.max(primaryTargetOpacity, secondaryTargetOpacity);
            
            PrimaryBarProgress = primaryProgress;
            PrimaryBarOpacity = UpdateOpacity(primaryBar.color.a, primaryTargetOpacity);
            
            SecondaryBarProgress = secondaryProgress;
            SecondaryBarOpacity = UpdateOpacity(secondaryBar.color.a, secondaryTargetOpacity);

            _wasJustAssigned = false;
        }

        protected abstract void GetPrimaryBarState(Entity entity, World world, EntityMonoBehaviour entityMono, out float progress, out bool visible);
        
        protected abstract void GetSecondaryBarState(Entity entity, World world, EntityMonoBehaviour entityMono, out float progress, out bool visible);
        
        private float UpdateProgress(float oldProgress, float newProgress, bool allowLerp) {
            var lerpSpeed = 0f;
            if (allowLerp) {
                if (lerpHealing && newProgress > oldProgress)
                    lerpSpeed = healingLerpSpeed;
                if (lerpLargeHits && oldProgress - newProgress >= largeHitThreshold)
                    lerpSpeed = largeHitLerpSpeed;
            }

            var progress = lerpSpeed > 0f ? math.lerp(oldProgress, newProgress, lerpSpeed * Time.deltaTime) : newProgress;
            if (math.abs(newProgress - progress) < 0.01f)
                progress = newProgress;

            return progress;
        }
        
        private float UpdateOpacity(float currentOpacity, float targetOpacity) {
            if (Manager.prefs.hideInGameUI || (Manager.main.player != null && Manager.main.player.guestMode))
                return 0f;
            
            var newOpacity = math.lerp(currentOpacity, targetOpacity, opacityLerpSpeed * Time.deltaTime);
            if (math.abs(targetOpacity - newOpacity) < 0.05f)
                newOpacity = targetOpacity;

            return newOpacity;
        }
        
        private void OnValidate() {
            var scale = new Vector2(barWidth / 16f, 2f / 16f);
            if (primaryBar == null || primaryBar.size == scale)
                return;
            
            primaryBar.size = scale;
            primaryBarBackground.size = new Vector2(scale.x + (2f / 16f), scale.y + (2f / 16f));
            primaryBarMaskPivot.transform.localPosition = new Vector3((0f - barWidth) * 0.0625f * 0.5f, 0f, 0f);
            primaryBar.transform.localPosition = new Vector3(barWidth * 0.0625f * 0.5f, 0f, 0f);
            
            secondaryBar.size = scale;
            secondaryBarBackground.size = new Vector2(scale.x + (2f / 16f), scale.y + (2f / 16f));
            secondaryBarMaskPivot.transform.localPosition = secondaryBarMaskPivot.transform.localPosition;
            secondaryBar.transform.localPosition = new Vector3(Mathf.Abs(secondaryBarMaskPivot.transform.localPosition.x), 0f, 0f);
        }
    }
}