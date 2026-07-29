using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFlowController : MonoBehaviour
{
    [Header("Testing")]
    [SerializeField, Min(1)] private int startingZone = 1;

    [Header("Wheel Data")]
    [SerializeField] private WheelData bronzeWheel;
    [SerializeField] private WheelData silverWheel;
    [SerializeField] private WheelData goldenWheel;

    [Header("Game References")]
    [SerializeField] private WheelView wheelView;
    [SerializeField] private WheelSpinController wheelSpinController;

    [Header("UI References")]
    [SerializeField] private TMP_Text zoneTitleText;
    [SerializeField] private TMP_Text zoneValueText;
    [SerializeField] private Button collectButton;
    [SerializeField] private RewardPanelView rewardPanelView;
    [SerializeField] private RunResultPopupView resultPopupView;
    [SerializeField] private WheelResultView wheelResultView;

    private int currentZone;
    private bool isRunEnded;

    private readonly RewardCollection rewardCollection =
        new RewardCollection();

    public int CurrentZone => currentZone;

    private void OnEnable()
    {
        if (wheelSpinController != null)
        {
            wheelSpinController.SpinStarted +=
                HandleSpinStarted;

            wheelSpinController.SpinCompleted +=
                HandleSpinCompleted;
        }

        if (collectButton != null)
        {
            collectButton.onClick.AddListener(
                HandleCollectRequested
            );
        }

        if (resultPopupView != null)
        {
            resultPopupView.RestartRequested +=
                HandleRestartRequested;
        }
    }

    private void OnDisable()
    {
        if (wheelSpinController != null)
        {
            wheelSpinController.SpinStarted -=
                HandleSpinStarted;

            wheelSpinController.SpinCompleted -=
                HandleSpinCompleted;
        }

        if (collectButton != null)
        {
            collectButton.onClick.RemoveListener(
                HandleCollectRequested
            );
        }

        if (resultPopupView != null)
        {
            resultPopupView.RestartRequested -=
                HandleRestartRequested;
        }
    }

    private void Start()
    {
        currentZone = Mathf.Max(1, startingZone);
        isRunEnded = false;

        if (resultPopupView != null)
        {
            resultPopupView.Hide();
        }

        if (wheelResultView != null)
        {
            wheelResultView.Hide();
        }

        if (wheelSpinController != null)
        {
            wheelSpinController.SetInteractionEnabled(true);
        }

        ApplyCurrentZone();
        RefreshRewardPanel();
    }

        private void HandleSpinStarted()
        {
            if (collectButton != null)
            {
                collectButton.interactable = false;
            }

            if (wheelResultView != null)
            {
                wheelResultView.Hide();
            }
        }

    private void HandleSpinCompleted(
        WheelSliceData selectedSlice
    )
    {
        if (isRunEnded)
        {
            return;
        }

        if (selectedSlice == null ||
            selectedSlice.Reward == null)
        {
            Debug.LogWarning(
                "Selected wheel slice has no reward.",
                this
            );

            ApplyCurrentZone();
            return;
        }

        RewardData selectedReward =
            selectedSlice.Reward;

        if (selectedReward.IsBomb)
        {
            HandleBomb();
            return;
        }

        int rewardAmount =
            selectedSlice.GetRewardAmount(currentZone);

        rewardCollection.Add(
            selectedReward,
            rewardAmount
        );

        RefreshRewardPanel();

        if (wheelResultView != null)
        {
            wheelResultView.Show(
                selectedReward,
                rewardAmount
            );
        }

        Debug.Log(
            $"Zone {currentZone} reward: " +
            $"{selectedReward.DisplayName} x{rewardAmount}"
        );

        currentZone++;

        ApplyCurrentZone();
    }

    private void HandleCollectRequested()
    {
        if (isRunEnded)
        {
            return;
        }

        if (wheelSpinController != null &&
            wheelSpinController.IsSpinning)
        {
            return;
        }

        ZoneType zoneType =
            ZoneService.GetZoneType(currentZone);

        bool canCollect =
            zoneType == ZoneType.Safe ||
            zoneType == ZoneType.Super;

        if (!canCollect)
        {
            Debug.LogWarning(
                "Rewards can only be collected in a Safe or Super Zone.",
                this
            );

            return;
        }

        if (resultPopupView == null)
        {
            Debug.LogWarning(
                "Result popup reference is missing.",
                this
            );

            return;
        }

        isRunEnded = true;

        if (wheelSpinController != null)
        {
            wheelSpinController.SetInteractionEnabled(false);
        }

        if (collectButton != null)
        {
            collectButton.interactable = false;
        }

        resultPopupView.ShowCollected(currentZone);

        Debug.Log(
            $"The player safely collected the rewards in Zone {currentZone}."
        );
    }

    private void HandleBomb()
    {
        isRunEnded = true;

        if (wheelResultView != null)
        {
            wheelResultView.Hide();
        }

        rewardCollection.Clear();
        RefreshRewardPanel();

        if (wheelSpinController != null)
        {
            wheelSpinController.SetInteractionEnabled(false);
        }

        if (collectButton != null)
        {
            collectButton.interactable = false;
        }

        if (resultPopupView != null)
        {
            resultPopupView.ShowBomb(currentZone);
        }

        Debug.Log(
            $"Bomb selected in Zone {currentZone}. " +
            "All collected rewards were lost."
        );
    }

    private void HandleRestartRequested()
    {
        currentZone = 1;
        isRunEnded = false;

        rewardCollection.Clear();
        RefreshRewardPanel();

        if (wheelResultView != null)
        {
            wheelResultView.Hide();
        }

        if (resultPopupView != null)
        {
            resultPopupView.Hide();
        }

        if (wheelSpinController != null)
        {
            wheelSpinController.ResetWheelRotation();
            wheelSpinController.SetInteractionEnabled(true);
        }

        ApplyCurrentZone();

        Debug.Log("The run restarted from Zone 1.");
    }

    private void ApplyCurrentZone()
    {
        ZoneType zoneType =
            ZoneService.GetZoneType(currentZone);

        WheelData selectedWheel =
            GetWheelData(zoneType);

        if (selectedWheel != null &&
            wheelView != null)
        {
            wheelView.SetWheelData(selectedWheel);
        }

        UpdateZoneUI(zoneType);
    }

    private WheelData GetWheelData(
        ZoneType zoneType
    )
    {
        switch (zoneType)
        {
            case ZoneType.Safe:
                return silverWheel;

            case ZoneType.Super:
                return goldenWheel;

            default:
                return bronzeWheel;
        }
    }

    private void UpdateZoneUI(ZoneType zoneType)
    {
        if (zoneValueText != null)
        {
            zoneValueText.text =
                $"ZONE {currentZone}";
        }

        if (zoneTitleText != null)
        {
            switch (zoneType)
            {
                case ZoneType.Safe:
                    zoneTitleText.text = "SAFE ZONE";
                    break;

                case ZoneType.Super:
                    zoneTitleText.text = "SUPER ZONE";
                    break;

                default:
                    zoneTitleText.text = "CURRENT ZONE";
                    break;
            }
        }

        if (collectButton != null)
        {
            bool isSafeZone =
                zoneType == ZoneType.Safe ||
                zoneType == ZoneType.Super;

            bool isWheelIdle =
                wheelSpinController == null ||
                !wheelSpinController.IsSpinning;

            collectButton.interactable =
                !isRunEnded &&
                isSafeZone &&
                isWheelIdle;
        }
    }

    private void RefreshRewardPanel()
    {
        if (rewardPanelView != null)
        {
            rewardPanelView.Refresh(
                rewardCollection.Entries
            );
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (wheelView == null)
        {
            wheelView =
                FindObjectOfType<WheelView>();
        }

        if (wheelSpinController == null)
        {
            wheelSpinController =
                FindObjectOfType<WheelSpinController>();
        }

        if (rewardPanelView == null)
        {
            rewardPanelView =
                FindObjectOfType<RewardPanelView>();
        }

        if (resultPopupView == null)
        {
            resultPopupView =
                FindObjectOfType<RunResultPopupView>(true);
        }

        if (zoneTitleText == null)
        {
            GameObject titleObject =
                GameObject.Find("ui_text_zone_title");

            if (titleObject != null)
            {
                zoneTitleText =
                    titleObject.GetComponent<TMP_Text>();
            }
        }

        if (zoneValueText == null)
        {
            GameObject valueObject =
                GameObject.Find("ui_text_zone_value");

            if (valueObject != null)
            {
                zoneValueText =
                    valueObject.GetComponent<TMP_Text>();
            }
        }

        if (collectButton == null)
        {
            GameObject collectObject =
                GameObject.Find("ui_button_collect");

            if (collectObject != null)
            {
                collectButton =
                    collectObject.GetComponent<Button>();
            }
        }

        if (wheelResultView == null)
        {
            wheelResultView =
                FindObjectOfType<WheelResultView>(true);
        }
    }
#endif
}