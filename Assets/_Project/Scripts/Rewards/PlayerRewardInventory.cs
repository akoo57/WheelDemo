using System;
using System.Collections.Generic;
using WheelDemo.Data;

namespace WheelDemo.Rewards
{
    public sealed class PlayerRewardInventory :
        IPlayerRewardInventory
    {
        private readonly RewardCollection rewards =
            new RewardCollection();

        public event Action InventoryChanged;

        public IEnumerable<RewardCollection.Entry> Entries =>
            rewards.Entries;

        public void AddReward(
            RewardData reward,
            int amount
        )
        {
            rewards.Add(reward, amount);
            InventoryChanged?.Invoke();
        }
    }
}
