using UnityEngine;
using WheelDemo.Data;

namespace WheelDemo.Rewards
{
    public abstract class RewardBehavior : ScriptableObject
    {
        public abstract bool ProducesAmount { get; }
        public abstract bool IsHazard { get; }

        public abstract void Resolve(
            IRewardResolutionContext context,
            RewardData reward,
            int amount
        );
    }
}
