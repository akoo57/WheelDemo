using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelDemo.Data;
using WheelDemo.UI;

namespace WheelDemo.Wheel
{
    public class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private Image rewardIconImage;
        [SerializeField] private TMP_Text rewardAmountText;

        private Quaternion uprightWorldRotation;

        public WheelSliceData SliceData { get; private set; }

        private void Awake()
        {
            uprightWorldRotation = transform.rotation;
        }

        public void Setup(WheelSliceData sliceData)
        {
            SetupIcon(sliceData);

            if (rewardAmountText != null)
            {
                rewardAmountText.gameObject.SetActive(false);
            }
        }

        public void Setup(WheelSliceData sliceData, int zoneNumber)
        {
            SetupIcon(sliceData);

            if (rewardAmountText == null)
            {
                return;
            }

            bool showsAmount =
                sliceData != null &&
                sliceData.Reward != null &&
                sliceData.Reward.ProducesAmount;

            rewardAmountText.gameObject.SetActive(showsAmount);

            if (showsAmount)
            {
                RewardVisualUtility.SetAmount(
                    rewardAmountText,
                    sliceData.GetRewardAmount(zoneNumber)
                );
            }
        }

        private void LateUpdate()
        {
            transform.rotation = uprightWorldRotation;
        }

        private void SetupIcon(WheelSliceData sliceData)
        {
            SliceData = sliceData;

            if (sliceData == null || sliceData.Reward == null)
            {
                Clear();
                return;
            }

            RewardData reward = sliceData.Reward;

            RewardVisualUtility.ApplyIcon(rewardIconImage, reward);
        }

        public void Clear()
        {
            SliceData = null;

            if (rewardIconImage == null)
            {
                return;
            }

            RewardVisualUtility.ApplyIcon(rewardIconImage, null);

            if (rewardAmountText != null)
            {
                RewardVisualUtility.SetAmount(rewardAmountText, 0);
                rewardAmountText.gameObject.SetActive(false);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rewardIconImage == null)
            {
                rewardIconImage = GetComponentInChildren<Image>(true);
            }

            if (rewardAmountText == null)
            {
                rewardAmountText = GetComponentInChildren<TMP_Text>(true);
            }
        }
#endif
    }
}
