using System.Collections.Generic;
using WheelDemo.Data;

namespace WheelDemo.Rewards
{
    public interface IPlayerRewardInventory
    {
        IEnumerable<RewardCollection.Entry> Entries { get; }

        void AddReward(RewardData reward, int amount);
    }
}
