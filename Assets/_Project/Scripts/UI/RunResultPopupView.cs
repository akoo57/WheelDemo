using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WheelDemo.UI
{
    public class RunResultPopupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text resultTitleText;
        [SerializeField] private TMP_Text resultMessageText;
        [SerializeField] private TMP_Text reviveCostText;

        [SerializeField] private Button reviveButton;
        [SerializeField] private Button restartButton;

        [SerializeField] private RectTransform reviveButtonRect;
        [SerializeField] private RectTransform restartButtonRect;

        [SerializeField]
        private Vector2 twoButtonRevivePosition = new Vector2(-110f, 30f);

        [SerializeField]
        private Vector2 twoButtonRestartPosition = new Vector2(110f, 30f);

        [SerializeField]
        private Vector2 singleRestartPosition = new Vector2(0f, 30f);

        public event Action ReviveRequested;
        public event Action RestartRequested;

        private void OnEnable()
        {
            if (reviveButton != null)
            {
                reviveButton.onClick.AddListener(HandleReviveButton);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(HandleRestartButton);
            }
        }

        private void OnDisable()
        {
            if (reviveButton != null)
            {
                reviveButton.onClick.RemoveListener(HandleReviveButton);
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(HandleRestartButton);
            }
        }

        public void ShowBombDecision(
            int zoneNumber,
            int reviveCost,
            bool canAfford
        )
        {
            if (resultTitleText != null)
            {
                resultTitleText.text = "BOMB!";
            }

            if (resultMessageText != null)
            {
                resultMessageText.text =
                    $"You hit a bomb in Zone {zoneNumber}.\n" +
                    "Revive to keep this run or restart from Zone 1.";
            }

            if (reviveCostText != null)
            {
                reviveCostText.text =
                    $"Revive Cost: {reviveCost} Gold";

                reviveCostText.gameObject.SetActive(true);
            }

            ApplyTwoButtonLayout();

            if (reviveButton != null)
            {
                reviveButton.interactable = canAfford;
            }

            gameObject.SetActive(true);
        }

        public void ShowCollected(int zoneNumber)
        {
            if (resultTitleText != null)
            {
                resultTitleText.text = "REWARDS COLLECTED!";
            }

            if (resultMessageText != null)
            {
                resultMessageText.text =
                    $"You safely exited in Zone {zoneNumber}.\n" +
                    "Your collected rewards were secured.";
            }

            if (reviveCostText != null)
            {
                reviveCostText.gameObject.SetActive(false);
            }

            ApplySingleRestartLayout();

            gameObject.SetActive(true);
        }

        public void ShowInsufficientGold(
            int zoneNumber,
            int reviveCost,
            int currentBalance
        )
        {
            if (resultTitleText != null)
            {
                resultTitleText.text = "NOT ENOUGH GOLD";
            }

            if (resultMessageText != null)
            {
                resultMessageText.text =
                    $"Zone {zoneNumber} is still active.\n" +
                    $"You need {reviveCost} Gold but only have {currentBalance}.";
            }

            if (reviveCostText != null)
            {
                reviveCostText.text =
                    $"Revive Cost: {reviveCost} Gold";

                reviveCostText.gameObject.SetActive(true);
            }

            ApplyTwoButtonLayout();

            if (reviveButton != null)
            {
                reviveButton.interactable = false;
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetReviveAvailability(bool canAfford)
        {
            if (reviveButton != null)
            {
                reviveButton.interactable = canAfford;
            }
        }

        private void ApplyTwoButtonLayout()
        {
            if (reviveButton != null)
            {
                reviveButton.gameObject.SetActive(true);
            }

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }

            if (reviveButtonRect != null)
            {
                reviveButtonRect.anchoredPosition =
                    twoButtonRevivePosition;
            }

            if (restartButtonRect != null)
            {
                restartButtonRect.anchoredPosition =
                    twoButtonRestartPosition;
            }
        }

        private void ApplySingleRestartLayout()
        {
            if (reviveButton != null)
            {
                reviveButton.gameObject.SetActive(false);
            }

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }

            if (restartButtonRect != null)
            {
                restartButtonRect.anchoredPosition =
                    singleRestartPosition;
            }
        }

        private void HandleReviveButton()
        {
            ReviveRequested?.Invoke();
        }

        private void HandleRestartButton()
        {
            RestartRequested?.Invoke();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (resultTitleText == null)
            {
                Transform titleTransform = transform.Find(
                    "ui_panel_result_card/ui_text_result_title"
                );

                if (titleTransform != null)
                {
                    resultTitleText =
                        titleTransform.GetComponent<TMP_Text>();
                }
            }

            if (resultMessageText == null)
            {
                Transform messageTransform = transform.Find(
                    "ui_panel_result_card/ui_text_result_message"
                );

                if (messageTransform != null)
                {
                    resultMessageText =
                        messageTransform.GetComponent<TMP_Text>();
                }
            }

            if (reviveCostText == null)
            {
                Transform costTransform = transform.Find(
                    "ui_panel_result_card/ui_text_revive_cost"
                );

                if (costTransform != null)
                {
                    reviveCostText =
                        costTransform.GetComponent<TMP_Text>();
                }
            }

            if (reviveButton == null)
            {
                Transform reviveTransform = transform.Find(
                    "ui_panel_result_card/ui_button_revive"
                );

                if (reviveTransform != null)
                {
                    reviveButton =
                        reviveTransform.GetComponent<Button>();
                }
            }

            if (restartButton == null)
            {
                Transform restartTransform = transform.Find(
                    "ui_panel_result_card/ui_button_restart"
                );

                if (restartTransform != null)
                {
                    restartButton =
                        restartTransform.GetComponent<Button>();
                }
            }

            if (reviveButtonRect == null &&
                reviveButton != null)
            {
                reviveButtonRect =
                    reviveButton.GetComponent<RectTransform>();
            }

            if (restartButtonRect == null &&
                restartButton != null)
            {
                restartButtonRect =
                    restartButton.GetComponent<RectTransform>();
            }
        }
#endif
    }
}