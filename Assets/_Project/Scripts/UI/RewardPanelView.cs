using System.Collections.Generic;
using UnityEngine;
using WheelDemo.Rewards;

namespace WheelDemo.UI
{
    public class RewardPanelView : MonoBehaviour
    {
        [SerializeField] private RectTransform contentRoot;
        [SerializeField] private RewardItemView rewardItemPrefab;

        private readonly List<RewardItemView> pooledItems =
            new List<RewardItemView>();

        public void Refresh(IEnumerable<RewardCollection.Entry> entries)
        {
            if (entries == null || rewardItemPrefab == null || contentRoot == null)
            {
                HideAllItems();
                return;
            }

            int itemIndex = 0;

            foreach (RewardCollection.Entry entry in entries)
            {
                RewardItemView itemView =
                    GetOrCreateItem(itemIndex);
                itemView.Setup(entry.Reward, entry.Amount);
                itemIndex++;
            }

            for (int i = itemIndex; i < pooledItems.Count; i++)
            {
                if (pooledItems[i] != null)
                {
                    pooledItems[i].gameObject.SetActive(false);
                }
            }
        }

        private RewardItemView GetOrCreateItem(int index)
        {
            while (pooledItems.Count <= index)
            {
                RewardItemView itemView =
                    Instantiate(rewardItemPrefab, contentRoot);

                pooledItems.Add(itemView);
            }

            RewardItemView pooledItem = pooledItems[index];

            if (pooledItem != null)
            {
                pooledItem.gameObject.SetActive(true);
            }

            return pooledItem;
        }

        private void HideAllItems()
        {
            for (int i = 0; i < pooledItems.Count; i++)
            {
                if (pooledItems[i] != null)
                {
                    pooledItems[i].gameObject.SetActive(false);
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (contentRoot == null)
            {
                Transform contentTransform =
                    transform.Find("ui_group_rewards_content");

                if (contentTransform != null)
                {
                    contentRoot =
                        contentTransform.GetComponent<RectTransform>();
                }
            }
        }
#endif
    }
}
