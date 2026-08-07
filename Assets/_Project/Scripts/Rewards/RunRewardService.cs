using System;
using System.Collections.Generic;
using WheelDemo.Data;

namespace WheelDemo.Rewards
{
    public sealed class RunRewardService
    {
        private readonly RewardCollection rewards =
            new RewardCollection();

        public event Action RewardsChanged;

        public IEnumerable<RewardCollection.Entry> Entries =>
            rewards.Entries;

        public void AddReward(
            RewardData reward,
            int amount
        )
        {
            rewards.Add(reward, amount);
            RewardsChanged?.Invoke();
        }

        public void Clear()
        {
            rewards.Clear();
            RewardsChanged?.Invoke();
        }
    }
}
