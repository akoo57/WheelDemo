using UnityEngine;
using WheelDemo.Data;

namespace WheelDemo.Rewards
{
    [CreateAssetMenu(
        fileName = "RewardBehavior_Standard",
        menuName = "Wheel Demo/Reward Behaviors/Standard"
    )]
    public sealed class StandardRewardBehavior : RewardBehavior
    {
        public override bool ProducesAmount => true;
        public override bool IsHazard => false;

        public override void Resolve(
            IRewardResolutionContext context,
            RewardData reward,
            int amount
        )
        {
            if (context == null || reward == null)
            {
                return;
            }

            context.GrantReward(reward, amount);
        }
    }
}
