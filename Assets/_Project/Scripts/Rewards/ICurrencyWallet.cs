using System;

namespace WheelDemo.Rewards
{
    public interface ICurrencyWallet
    {
        event Action<int> BalanceChanged;

        int CurrentBalance { get; }

        bool CanAfford(int amount);

        bool TrySpend(int amount);

        void Add(int amount);
    }
}
