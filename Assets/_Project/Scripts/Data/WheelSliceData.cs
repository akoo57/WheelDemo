using System;
using UnityEngine;

[Serializable]
public class WheelSliceData
{
    [SerializeField] private RewardData reward;
    [SerializeField, Min(1)] private int amountMultiplier = 1;

    public RewardData Reward => reward;
    public int AmountMultiplier => amountMultiplier;

    public int GetRewardAmount(int zoneNumber)
    {
        if (reward == null || reward.IsBomb)
        {
            return 0;
        }

        int safeZoneNumber = Mathf.Max(1, zoneNumber);

        int progressionMultiplier =
            1 + (safeZoneNumber - 1) / 5;

        return reward.BaseAmount *
            amountMultiplier *
            progressionMultiplier;
    }
}