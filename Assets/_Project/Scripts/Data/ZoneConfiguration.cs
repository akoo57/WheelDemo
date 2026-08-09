using System;
using UnityEngine;
using WheelDemo.Core;

namespace WheelDemo.Data
{
    [Serializable]
    public sealed class ZoneDefinition
    {
        [SerializeField] private ZoneType zoneType;
        [SerializeField] private WheelData wheelData;
        [SerializeField] private bool allowsCollection;

        public ZoneType ZoneType => zoneType;
        public WheelData WheelData => wheelData;
        public bool AllowsCollection => allowsCollection;

        public ZoneDefinition()
        {
        }

    }

    [CreateAssetMenu(
        fileName = "ZoneConfiguration_New",
        menuName = "Wheel Demo/Zone Configuration"
    )]
    public sealed class ZoneConfiguration : ScriptableObject
    {
        [Header("Progression")]
        [SerializeField, Min(1)] private int safeZoneInterval = 5;
        [SerializeField, Min(1)] private int superZoneInterval = 30;

        [Header("Definitions")]
        [SerializeField] private ZoneDefinition normalZone =
            new ZoneDefinition();

        [SerializeField] private ZoneDefinition safeZone =
            new ZoneDefinition();

        [SerializeField] private ZoneDefinition superZone =
            new ZoneDefinition();

        public int SafeZoneInterval => safeZoneInterval;
        public int SuperZoneInterval => superZoneInterval;

        public bool TryGetDefinition(
            int zoneNumber,
            out ZoneDefinition definition
        )
        {
            definition = null;

            if (zoneNumber < 1)
            {
                return false;
            }

            if (superZoneInterval > 0 &&
                zoneNumber % superZoneInterval == 0)
            {
                definition = superZone;
            }
            else if (safeZoneInterval > 0 &&
                zoneNumber % safeZoneInterval == 0)
            {
                definition = safeZone;
            }
            else
            {
                definition = normalZone;
            }

            return definition != null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            safeZoneInterval = Mathf.Max(1, safeZoneInterval);
            superZoneInterval = Mathf.Max(1, superZoneInterval);

            if (superZoneInterval % safeZoneInterval != 0)
            {
                Debug.LogWarning(
                    "The Super Zone interval should also be a Safe Zone interval.",
                    this
                );
            }

            ValidateDefinition(normalZone, ZoneType.Normal, "Normal");
            ValidateDefinition(safeZone, ZoneType.Safe, "Safe");
            ValidateDefinition(superZone, ZoneType.Super, "Super");
        }

        private void ValidateDefinition(
            ZoneDefinition definition,
            ZoneType expectedType,
            string label
        )
        {
            if (definition == null)
            {
                Debug.LogWarning($"{label} Zone definition is missing.", this);
                return;
            }

            if (definition.ZoneType != expectedType)
            {
                Debug.LogWarning(
                    $"{label} Zone definition uses {definition.ZoneType} type.",
                    this
                );
            }

            if (definition.WheelData == null)
            {
                Debug.LogWarning(
                    $"{label} Zone definition has no wheel data.",
                    this
                );
            }
        }
#endif
    }
}
