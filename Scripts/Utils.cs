using System;
using System.Collections.Generic;
using System.Globalization;
using HealthBars.Scripts.Bars;
using HealthBars.Scripts.Bars.Assignment;
using HealthBars.Scripts.Patches;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HealthBars.Scripts {
    public static class Utils {
        private static readonly Dictionary<ResourceBarType, Type> BarTypeToType = new();
        private static readonly Dictionary<ObjectID, Vector3> BarOffsets = new() {
            { ObjectID.CrystalBigSnail, new Vector3(0f, 0f, -0.75f) },
            { ObjectID.SnarePlant, new Vector3(0f, 0f, -0.35f) },
            { ObjectID.SmallTentacle, new Vector3(0f, 0f, -0.275f) },
            { ObjectID.MoldTentacle, new Vector3(0f, 0f, -0.45f) },
            { ObjectID.CrystalMerchant, new Vector3(0f, 0f, 0.2f) },
            { ObjectID.BombScarab, new Vector3(0f, 0f, 0.1f) },
            { ObjectID.LavaButterfly, new Vector3(0f, 0f, 0.45f) },
            { ObjectID.Larva, new Vector3(0f, 0f, -0.3f) },
            { ObjectID.CrabEnemy, new Vector3(0f, 0f, 0.15f) },
            { ObjectID.OrbitalTurret, new Vector3(0f, 0f, 0.1f) },
            { ObjectID.OctopusTentacle, new Vector3(0.4f, 0f, 0.1f) },
            { ObjectID.BirdBossStone, new Vector3(0f, 0f, -0.3f) },
            { ObjectID.CavelingMerchant, new Vector3(-0.15f, 0f, 0f) },
            { ObjectID.MeleeMinion, new Vector3(0.1f, 0f, 0f) },
            { ObjectID.BatMinion, new Vector3(0f, 0f, 0.5f) },
            { ObjectID.LavaSlimeBlob, new Vector3(0f, 0f, -0.1f) },
            { ObjectID.WormSegment, new Vector3(0f, 0f, -0.6f) },
            { ObjectID.ClayWormSegment, new Vector3(0f, 0f, -0.4f) },
            { ObjectID.NatureWormSegment, new Vector3(0f, 0f, -0.4f) },
            { ObjectID.AmoebaWormSegment, new Vector3(0f, 0f, -0.4f) }
        };
        
        public static void RegisterResourceBar(ResourceBarBase barBase) {
            MemoryManager_Patch.BarPrefabsToRegister.Add(barBase.gameObject);
            BarTypeToType[barBase.type] = barBase.GetType();
        }
        
        public static Component GetFreeResourceBarComponent(ResourceBarType type) {
            return Manager.memory.GetFreeComponent(BarTypeToType[type], true, true);
        }
        
        public static Vector3 GetBottom(EntityMonoBehaviour entityMono) {
            var bottom = entityMono.RenderPosition;
            if (entityMono.objectInfo.centerIsAtEntityPosition)
                return bottom;
            
            EntityUtility.GetPrefabSizeAndOffset(entityMono.entity, entityMono.objectInfo, out var size, out var offset);
                
            var actualSize = size - offset;
            if (actualSize.x > 1)
                bottom.x += (actualSize.x - 1) / 2f;

            return bottom;
        }
        
        public static void AssignResourceBar(Entity entity, EntityManager manager, GameObject graphicalObject) {
            if (graphicalObject == null || !manager.HasComponent<HasResourceBarCD>(entity) || !manager.HasComponent<ObjectDataCD>(entity))
                return;
                
            var entityMono = graphicalObject.GetComponent<EntityMonoBehaviour>();
            if (entityMono != null && entityMono.optionalHealthBar != null)
                return;
                
            var objectData = manager.GetComponentData<ObjectDataCD>(entity);
            var barType = manager.GetComponentData<HasResourceBarCD>(entity).Type;
                
            var barHandler = graphicalObject.GetComponent<ResourceBarHandler>();
            if (barHandler == null)
                barHandler = graphicalObject.AddComponent<ResourceBarHandler>();

            EntityMonoBehaviour_Patch.BarHandlerLookup[graphicalObject.GetInstanceID()] = barHandler;

            var barInstance = (ResourceBarBase) GetFreeResourceBarComponent(barType);
            if (barInstance == null) {
                Debug.Log("Tried to assign a resource bar, but one wasn't free (this shouldn't happen)");
                return;
            }

            barHandler.bar = barInstance;
            barInstance.transform.SetParent(graphicalObject.transform, false);
            var offset = GetBottom(entityMono) - entityMono.RenderPosition + BarOffsets.GetValueOrDefault(objectData.objectID) + new Vector3(0f, 1.6f, -1.6f);
            barInstance.transform.localPosition = offset;
            barInstance.transform.localScale = Vector3.one;
            barInstance.transform.localRotation = Quaternion.identity;
            barInstance.OnOccupied();


        }

        public static void UnassignResourceBar(Entity entity, EntityManager manager, GameObject graphicalObject) {
            if (graphicalObject == null)
                return;
                
            var barHandler = graphicalObject.GetComponent<ResourceBarHandler>();
            if (barHandler == null || barHandler.bar == null)
                return;

            EntityMonoBehaviour_Patch.BarHandlerLookup.Remove(graphicalObject.GetInstanceID());

            barHandler.bar.Free();
            barHandler.bar = null;
        }

        public static bool TryGetNormalizedHealth(Entity entity, World world, EntityMonoBehaviour entityMono, out float health) {
            health = 0f;
            if (!EntityUtility.TryGetComponentData<HealthCD>(entity, world, out var healthData))
                return false;
            
            var currentHealth = entityMono.GetCurrentHealth(in healthData);
            var maxHealth = entityMono.GetMaxHealth();
                
            health = math.clamp(currentHealth / (float) maxHealth, 0f, 1f);
            return true;
        }
        
        public static bool TryGetNormalizedMana(Entity entity, World world, PlayerController playerController, out float mana) {
            mana = 0f;
            if (!EntityUtility.TryGetComponentData<ManaCD>(entity, world, out var manaData))
                return false;
            
            mana = math.clamp(manaData.mana / (float) manaData.maxMana, 0f, 1f);
            return true;
        }
        
        public static Color? ParseHexColor(string hexColor) {
            if (string.IsNullOrEmpty(hexColor) || hexColor.Length < 7)
                return null;

            hexColor = hexColor.Replace("#", "");

            return new Color(
                byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber) / 255f,
                byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber) / 255f,
                byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber) / 255f,
                1f
            );
        }
    }
}