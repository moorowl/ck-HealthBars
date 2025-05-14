using Pug.UnityExtensions;
using Unity.Entities;
using UnityEngine;

namespace HealthBars.Scripts.Bars {
    public abstract class ResourceBarBase : MonoBehaviour, IPoolable {
        public ResourceBarType type;
        
        public abstract void UpdateState(Entity entity, World world, EntityMonoBehaviour entityMono);
        
        #region Pooling stuff
        private IPoolSystem _pool;

        public bool IsPooled => _pool != null;
        public bool IsFree => _pool.IsFree(gameObject);

        public void OnAllocation(IPoolSystem pool) {
            _pool = pool;
        }

        public virtual void OnOccupied() { }

        public virtual void OnFree() { }

        public virtual void Free() {
            if (IsPooled) {
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