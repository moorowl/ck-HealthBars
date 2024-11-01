using System;
using PugConversion;
using UnityEngine;

namespace HealthBars.Scripts.Bars.Assignment {
    public class ResourceBarConverter : PugConverter {
        public override void Convert(GameObject authoring) {
            if (IsServer)
                return;

            if (!authoring.TryGetComponent<IEntityMonoBehaviourData>(out var entityMonoBehaviourData))
                return;

            var objectInfo = entityMonoBehaviourData.ObjectInfo;
            var graphicalPrefab = objectInfo.prefabInfos[0].prefab?.gameObject;
            if (graphicalPrefab == null)
                return;
            
            var entityMono = graphicalPrefab.GetComponent<EntityMonoBehaviour>();
            if (entityMono == null || entityMono.optionalHealthBar != null)
                return;

            var barType = GetResourceBarType(authoring, entityMono, objectInfo);
            if (barType == null || !Options.ShouldShowResourceBar(barType.Value))
                return;
            
            AddComponentData(new HasResourceBarCD {
                Type = barType.Value
            });
        }

        private static ResourceBarType? GetResourceBarType(GameObject authoring, EntityMonoBehaviour entityMono, ObjectInfo objectInfo) {
            if (entityMono is PlayerController)
                return ResourceBarType.Player;
            
            if (entityMono is CrystalBigSnail)
                return ResourceBarType.CrystalSnail;
            
            if (entityMono is OreBoulder)
                return ResourceBarType.OreBoulder;
            
            if (entityMono is MinionBase)
                return ResourceBarType.MinionDuration;

            if (authoring.TryGetComponent<HealthAuthoring>(out var healthAuthoring) && healthAuthoring.maxHealth > 1 &&
                objectInfo.objectType == ObjectType.Creature && objectInfo.objectID != ObjectID.AmoebaGiantSegment)
                return ResourceBarType.HealthAndArmor;
            
            if (objectInfo.objectID == ObjectID.BirdBossStone)
                return ResourceBarType.HealthAndArmor;
            
            return null;
        }
    }
}
