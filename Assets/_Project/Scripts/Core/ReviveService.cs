using WheelDemo.Rewards;

namespace WheelDemo.Core
{
    public sealed class ReviveService
    {
        private readonly ICurrencyWallet wallet;
        private readonly int reviveCost;

        public int CurrentBalance => wallet != null
            ? wallet.CurrentBalance
            : 0;

        public int ReviveCost => reviveCost;

        public bool IsAvailable => wallet != null &&
            wallet.CanAfford(reviveCost);

        public ReviveService(
            ICurrencyWallet wallet,
            int reviveCost
        )
        {
            this.wallet = wallet;
            this.reviveCost = reviveCost < 0 ? 0 : reviveCost;
        }

        public bool TryRevive()
        {
            return wallet != null &&
                wallet.TrySpend(reviveCost);
        }
    }
}
