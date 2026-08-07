using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelDemo.Data;

namespace WheelDemo.UI
{
    public static class RewardVisualUtility
    {
        public static void ApplyIcon(
            Image targetImage,
            RewardData reward
        )
        {
            if (targetImage == null)
            {
                return;
            }

            if (reward == null)
            {
                targetImage.sprite = null;
                targetImage.enabled = false;
                targetImage.rectTransform.localScale = Vector3.one;
                return;
            }

            targetImage.sprite = reward.Icon;
            targetImage.enabled = reward.Icon != null;
            targetImage.rectTransform.localScale =
                Vector3.one * reward.IconScale;
        }

        public static void SetAmount(
            TMP_Text amountText,
            int amount
        )
        {
            if (amountText == null)
            {
                return;
            }

            amountText.text = amount > 0
                ? $"x{amount}"
                : string.Empty;
        }
    }
}
