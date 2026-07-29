using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunResultPopupView : MonoBehaviour
{
    [SerializeField] private TMP_Text resultTitleText;
    [SerializeField] private TMP_Text resultMessageText;
    [SerializeField] private Button restartButton;

    public event Action RestartRequested;

    private void OnEnable()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(HandleRestartButton);
        }
    }

    private void OnDisable()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(HandleRestartButton);
        }
    }

    public void ShowBomb(int zoneNumber)
    {
        if (resultTitleText != null)
        {
            resultTitleText.text = "BOMB!";
        }

        if (resultMessageText != null)
        {
            resultMessageText.text =
                $"You hit a bomb in Zone {zoneNumber}.\n" +
                "All collected rewards were lost.";
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

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
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

        if (restartButton == null)
        {
            Transform buttonTransform = transform.Find(
                "ui_panel_result_card/ui_button_restart"
            );

            if (buttonTransform != null)
            {
                restartButton =
                    buttonTransform.GetComponent<Button>();
            }
        }
    }
#endif
}