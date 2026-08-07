using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelDemo.Data;

namespace WheelDemo.UI
{
    public class RewardItemView : MonoBehaviour
    {
        [SerializeField] private Image rewardIconImage;
        [SerializeField] private TMP_Text rewardAmountText;

        public void Setup(RewardData reward, int amount)
        {
            if (reward == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            RewardVisualUtility.ApplyIcon(rewardIconImage, reward);
            RewardVisualUtility.SetAmount(rewardAmountText, amount);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rewardIconImage == null)
            {
                Transform iconTransform =
                    transform.Find("ui_image_reward_icon");

                if (iconTransform != null)
                {
                    rewardIconImage =
                        iconTransform.GetComponent<Image>();
                }
            }

            if (rewardAmountText == null)
            {
                Transform amountTransform =
                    transform.Find("ui_text_reward_amount_value");

                if (amountTransform != null)
                {
                    rewardAmountText =
                        amountTransform.GetComponent<TMP_Text>();
                }
            }
        }
#endif
    }
}
