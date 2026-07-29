using UnityEngine;
using UnityEngine.UI;

public class WheelSliceView : MonoBehaviour
{
    [SerializeField] private Image rewardIconImage;

    public WheelSliceData SliceData { get; private set; }

    public void Setup(WheelSliceData sliceData)
    {
        SliceData = sliceData;

        if (sliceData == null || sliceData.Reward == null)
        {
            Clear();
            return;
        }

        RewardData reward = sliceData.Reward;

        rewardIconImage.sprite = reward.Icon;
        rewardIconImage.enabled = reward.Icon != null;

        rewardIconImage.rectTransform.localScale =
            Vector3.one * reward.IconScale;
    }

    public void Clear()
    {
        SliceData = null;

        if (rewardIconImage == null)
        {
            return;
        }

        rewardIconImage.sprite = null;
        rewardIconImage.enabled = false;
        rewardIconImage.rectTransform.localScale = Vector3.one;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (rewardIconImage == null)
        {
            rewardIconImage = GetComponentInChildren<Image>(true);
        }
    }
#endif
}