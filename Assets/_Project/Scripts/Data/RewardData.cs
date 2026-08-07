using UnityEngine;
using WheelDemo.Rewards;

namespace WheelDemo.Data
{
    [CreateAssetMenu(
        fileName = "RewardData_New",
        menuName = "Wheel Demo/Reward Data"
    )]
    public class RewardData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string rewardId;
        [SerializeField] private string displayName;
        [SerializeField] private RewardType rewardType;

        [Header("Visual")]
        [SerializeField] private Sprite icon;
        [SerializeField, Min(0.1f)] private float iconScale = 1f;

        [Header("Value")]
        [SerializeField, Min(0)] private int baseAmount = 1;
        [SerializeField] private RewardBehavior behavior;

        public string RewardId => rewardId;
        public string DisplayName => displayName;
        public RewardType RewardType => rewardType;
        public Sprite Icon => icon;
        public float IconScale => iconScale;
        public int BaseAmount => baseAmount;
        public RewardBehavior Behavior => behavior;
        public bool SettlesToCurrency => rewardType == RewardType.Gold;
        public bool ProducesAmount => behavior != null
            ? behavior.ProducesAmount
            : rewardType != RewardType.Bomb;
        public bool IsHazard => behavior != null
            ? behavior.IsHazard
            : rewardType == RewardType.Bomb;

        public bool IsBomb => IsHazard;

        public void Resolve(
            IRewardResolutionContext context,
            int amount
        )
        {
            if (context == null)
            {
                return;
            }

            if (behavior != null)
            {
                behavior.Resolve(context, this, amount);
                return;
            }

            if (rewardType == RewardType.Bomb)
            {
                context.TriggerBomb();
            }
            else
            {
                context.GrantReward(this, amount);
            }
        }
    }
}
