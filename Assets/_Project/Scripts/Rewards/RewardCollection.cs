using System.Collections.Generic;
using WheelDemo.Data;

namespace WheelDemo.Rewards
{
    public sealed class RewardCollection
    {
        public sealed class Entry
        {
            public RewardData Reward { get; private set; }
            public int Amount { get; private set; }

            public Entry(RewardData reward, int amount)
            {
                Reward = reward;
                Amount = amount;
            }

            public void AddAmount(int amount)
            {
                Amount += amount;
            }
        }

        private readonly Dictionary<RewardData, Entry> entriesByReward =
            new Dictionary<RewardData, Entry>();

        private readonly List<Entry> orderedEntries =
            new List<Entry>();

        public IEnumerable<Entry> Entries => orderedEntries;

        public void Add(RewardData reward, int amount)
        {
            if (reward == null || reward.IsHazard || amount <= 0)
            {
                return;
            }

            Entry existingEntry;

            if (entriesByReward.TryGetValue(reward, out existingEntry))
            {
                existingEntry.AddAmount(amount);
                return;
            }

            Entry newEntry = new Entry(reward, amount);

            entriesByReward.Add(reward, newEntry);
            orderedEntries.Add(newEntry);
        }

        public void Clear()
        {
            entriesByReward.Clear();
            orderedEntries.Clear();
        }
    }
}
