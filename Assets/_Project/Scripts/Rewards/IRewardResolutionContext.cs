using WheelDemo.Data;

namespace WheelDemo.Rewards
{
    public interface IRewardResolutionContext
    {
        void GrantReward(RewardData reward, int amount);
        void TriggerBomb();
    }
}
