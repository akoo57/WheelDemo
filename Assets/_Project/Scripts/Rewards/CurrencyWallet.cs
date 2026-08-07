using TMPro;
using UnityEngine;

namespace WheelDemo.Rewards
{
    public class CurrencyWallet : MonoBehaviour, ICurrencyWallet
    {
        [SerializeField, Min(0)] private int startingBalance;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private string balanceFormat = "{0}";

        public event System.Action<int> BalanceChanged;

        public int CurrentBalance { get; private set; }

        private void Awake()
        {
            CurrentBalance = Mathf.Max(0, startingBalance);
            NotifyBalanceChanged();
        }

        public bool CanAfford(int amount)
        {
            return amount <= 0 ||
                CurrentBalance >= amount;
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (!CanAfford(amount))
            {
                return false;
            }

            CurrentBalance -= amount;
            NotifyBalanceChanged();
            return true;
        }

        public void Add(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentBalance += amount;
            NotifyBalanceChanged();
        }

        private void NotifyBalanceChanged()
        {
            RefreshBalanceView();
            BalanceChanged?.Invoke(CurrentBalance);
        }

        private void RefreshBalanceView()
        {
            if (balanceText == null)
            {
                return;
            }

            balanceText.text = string.Format(
                balanceFormat,
                CurrentBalance
            );
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            startingBalance = Mathf.Max(0, startingBalance);
        }
#endif
    }
}
