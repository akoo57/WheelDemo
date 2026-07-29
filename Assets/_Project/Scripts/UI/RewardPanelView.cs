using System.Collections.Generic;
using UnityEngine;

public class RewardPanelView : MonoBehaviour
{
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private RewardItemView rewardItemPrefab;

    private readonly List<RewardItemView> spawnedItems =
        new List<RewardItemView>();

    public void Refresh(IEnumerable<RewardCollection.Entry> entries)
    {
        ClearSpawnedItems();

        if (entries == null || rewardItemPrefab == null || contentRoot == null)
        {
            return;
        }

        foreach (RewardCollection.Entry entry in entries)
        {
            RewardItemView itemView =
                Instantiate(rewardItemPrefab, contentRoot);

            itemView.Setup(entry.Reward, entry.Amount);
            spawnedItems.Add(itemView);
        }
    }

    private void ClearSpawnedItems()
    {
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if (spawnedItems[i] != null)
            {
                Destroy(spawnedItems[i].gameObject);
            }
        }

        spawnedItems.Clear();
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