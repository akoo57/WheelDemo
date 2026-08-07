using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelDemo.Data;

namespace WheelDemo.UI
{
    public class WheelResultView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image rewardIconImage;
        [SerializeField] private TMP_Text rewardAmountText;

        [Header("Animation")]
        [SerializeField, Min(0.05f)]
        private float animationDuration = 0.25f;

        private Coroutine animationCoroutine;

        public void Show(RewardData reward, int amount)
        {
            if (reward == null)
            {
                Hide();
                return;
            }

            gameObject.SetActive(true);

            RewardVisualUtility.ApplyIcon(rewardIconImage, reward);

            if (rewardAmountText != null)
            {
                rewardAmountText.text = reward.IsBomb
                    ? "BOMB!"
                    : string.Empty;

                if (!reward.IsBomb)
                {
                    RewardVisualUtility.SetAmount(
                        rewardAmountText,
                        amount
                    );
                }
            }

            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine =
                StartCoroutine(PlayShowAnimation());
        }

        public void Hide()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            transform.localScale = Vector3.one;
            gameObject.SetActive(false);
        }

        private IEnumerator PlayShowAnimation()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            transform.localScale =
                Vector3.one * 0.7f;

            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;

                float normalizedTime =
                    Mathf.Clamp01(elapsedTime / animationDuration);

                float easedTime =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        normalizedTime
                    );

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = easedTime;
                }

                transform.localScale =
                    Vector3.LerpUnclamped(
                        Vector3.one * 0.7f,
                        Vector3.one,
                        easedTime
                    );

                yield return null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            transform.localScale = Vector3.one;
            animationCoroutine = null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (canvasGroup == null)
            {
                canvasGroup =
                    GetComponent<CanvasGroup>();
            }

            if (rewardIconImage == null)
            {
                Transform iconTransform =
                    transform.Find(
                        "ui_image_spin_result_icon"
                    );

                if (iconTransform != null)
                {
                    rewardIconImage =
                        iconTransform.GetComponent<Image>();
                }
            }

            if (rewardAmountText == null)
            {
                Transform amountTransform =
                    transform.Find(
                        "ui_text_spin_result_amount_value"
                    );

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
