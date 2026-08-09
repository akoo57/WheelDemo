using System;
using UnityEngine;

namespace WheelDemo.Data
{
    [Serializable]
    public class WheelSliceData
    {
        [SerializeField] private RewardData reward;
        [SerializeField, Min(1)] private int amountMultiplier = 1;
        [SerializeField]
        private RewardProgressionConfiguration progressionConfiguration;

        public RewardData Reward => reward;
        public int AmountMultiplier => amountMultiplier;
        public RewardProgressionConfiguration ProgressionConfiguration =>
            progressionConfiguration;

        public int GetRewardAmount(int zoneNumber)
        {
            if (reward == null || !reward.ProducesAmount || progressionConfiguration == null)
            {
                return 0;
            }

            int safeZoneNumber = Mathf.Max(1, zoneNumber);

            return progressionConfiguration.CalculateAmount(
                reward.BaseAmount,
                amountMultiplier,
                safeZoneNumber
            );
        }
    }
}
