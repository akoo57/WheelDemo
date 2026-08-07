using System.Collections.Generic;

namespace WheelDemo.Rewards
{
    public sealed class RewardSettlementService
    {
        private readonly ICurrencyWallet currencyWallet;
        private readonly IPlayerRewardInventory playerRewardInventory;

        public RewardSettlementService(
            ICurrencyWallet currencyWallet,
            IPlayerRewardInventory playerRewardInventory
        )
        {
            this.currencyWallet = currencyWallet;
            this.playerRewardInventory = playerRewardInventory;
        }

        public bool TrySettle(
            IEnumerable<RewardCollection.Entry> entries
        )
        {
            if (entries == null ||
                currencyWallet == null ||
                playerRewardInventory == null)
            {
                return false;
            }

            foreach (RewardCollection.Entry entry in entries)
            {
                if (entry == null ||
                    entry.Reward == null ||
                    entry.Amount <= 0)
                {
                    continue;
                }

                if (entry.Reward.SettlesToCurrency)
                {
                    currencyWallet.Add(entry.Amount);
                    continue;
                }

                playerRewardInventory.AddReward(
                    entry.Reward,
                    entry.Amount
                );
            }

            return true;
        }
    }
}
