using Unity.Entities;
using UnityEngine;

namespace HealthBars.Scripts.Bars {
    public class ResourceBarHandler : MonoBehaviour {
        public ResourceBarBase bar;

        public void UpdateState(Entity entity, World world, EntityMonoBehaviour entityMono) {
            if (bar != null)
                bar.UpdateState(entity, world, entityMono);
        }
    }
}