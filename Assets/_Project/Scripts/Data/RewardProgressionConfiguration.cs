using UnityEngine;

namespace WheelDemo.Data
{
    [CreateAssetMenu(
        fileName = "RewardProgressionConfiguration_New",
        menuName = "Wheel Demo/Reward Progression Configuration"
    )]
    public sealed class RewardProgressionConfiguration : ScriptableObject
    {
        [SerializeField, Min(0f)] private float increasePerZone = 0.1f;

        public float IncreasePerZone => increasePerZone;

        public int CalculateAmount(
            int baseAmount,
            int multiplier,
            int zoneNumber
        )
        {
            int safeBaseAmount = Mathf.Max(0, baseAmount);
            int safeMultiplier = Mathf.Max(0, multiplier);
            int safeZoneNumber = Mathf.Max(1, zoneNumber);

            float progression =
                1f + (safeZoneNumber - 1) * increasePerZone;

            return Mathf.Max(
                0,
                Mathf.RoundToInt(
                    safeBaseAmount * safeMultiplier * progression
                )
            );
        }
    }
}
