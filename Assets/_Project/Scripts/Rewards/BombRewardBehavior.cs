using UnityEngine;
using WheelDemo.Data;

namespace WheelDemo.Rewards
{
    [CreateAssetMenu(
        fileName = "RewardBehavior_Bomb",
        menuName = "Wheel Demo/Reward Behaviors/Bomb"
    )]
    public sealed class BombRewardBehavior : RewardBehavior
    {
        public override bool ProducesAmount => false;
        public override bool IsHazard => true;

        public override void Resolve(
            IRewardResolutionContext context,
            RewardData reward,
            int amount
        )
        {
            if (context == null)
            {
                return;
            }

            context.TriggerBomb();
        }
    }
}
