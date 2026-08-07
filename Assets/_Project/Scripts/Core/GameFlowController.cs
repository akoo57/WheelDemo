using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelDemo.Data;
using WheelDemo.Rewards;
using WheelDemo.UI;
using WheelDemo.Wheel;

namespace WheelDemo.Core
{
    public class GameFlowController : MonoBehaviour, IRewardResolutionContext
    {
        [Header("Testing")]
        [SerializeField, Min(1)] private int startingZone = 1;
        [SerializeField, Min(0)] private int reviveCost = 50;

        [Header("Wheel Data")]
        [SerializeField] private WheelData bronzeWheel;
        [SerializeField] private WheelData silverWheel;
        [SerializeField] private WheelData goldenWheel;

        [Header("Zone Configuration")]
        [SerializeField] private ZoneConfiguration zoneConfiguration;

        [Header("Game References")]
        [SerializeField] private WheelView wheelView;
        [SerializeField] private WheelSpinController wheelSpinController;

        [Header("UI References")]
        [SerializeField] private TMP_Text zoneTitleText;
        [SerializeField] private TMP_Text zoneValueText;
        [SerializeField] private Button collectButton;
        [SerializeField] private CurrencyWallet currencyWallet;
        [SerializeField] private RewardPanelView rewardPanelView;
        [SerializeField] private RunResultPopupView resultPopupView;
        [SerializeField] private WheelResultView wheelResultView;

        private GameFlowStateMachine stateMachine;
        private GameInteractionPolicy interactionPolicy;
        private ZoneProgressionService zoneProgressionService;
        private ZoneConfiguration runtimeZoneConfiguration;
        private RunRewardService runRewardService;
        private IPlayerRewardInventory playerRewardInventory;
        private RewardSettlementService rewardSettlementService;
        private ReviveService reviveService;
        private RunFlowService runFlowService;
        private CurrencyWallet runtimeCurrencyWallet;

        public int CurrentZone => zoneProgressionService != null
            ? zoneProgressionService.CurrentZone
            : Mathf.Max(1, startingZone);

        private void Awake()
        {
            ZoneConfiguration activeConfiguration = zoneConfiguration;

            if (activeConfiguration == null)
            {
                runtimeZoneConfiguration =
                    ZoneConfiguration.CreateRuntime(
                        bronzeWheel,
                        silverWheel,
                        goldenWheel
                    );

                activeConfiguration = runtimeZoneConfiguration;
            }

            stateMachine = new GameFlowStateMachine(
                GameFlowState.ReadyToSpin
            );
            interactionPolicy = new GameInteractionPolicy(stateMachine);
            zoneProgressionService = new ZoneProgressionService(
                activeConfiguration,
                startingZone
            );
            runRewardService = new RunRewardService();
            playerRewardInventory = new PlayerRewardInventory();

            if (currencyWallet == null)
            {
                GameObject walletObject = new GameObject(
                    "RuntimeCurrencyWallet"
                );

                walletObject.hideFlags = HideFlags.HideAndDontSave;
                runtimeCurrencyWallet =
                    walletObject.AddComponent<CurrencyWallet>();
                currencyWallet = runtimeCurrencyWallet;
            }

            rewardSettlementService = new RewardSettlementService(
                currencyWallet,
                playerRewardInventory
            );
            reviveService = new ReviveService(
                currencyWallet,
                reviveCost
            );
            runFlowService = new RunFlowService(
                stateMachine,
                interactionPolicy,
                zoneProgressionService,
                rewardSettlementService,
                runRewardService,
                reviveService
            );
        }

        private void OnEnable()
        {
            if (stateMachine != null)
            {
                stateMachine.StateChanged += HandleStateChanged;
            }

            if (wheelSpinController != null)
            {
                wheelSpinController.SpinStarted +=
                    HandleSpinStarted;

                wheelSpinController.SpinCompleted +=
                    HandleSpinCompleted;

                wheelSpinController.SpinCancelled +=
                    HandleSpinCancelled;
            }

            if (collectButton != null)
            {
                collectButton.onClick.AddListener(
                    HandleCollectRequested
                );
            }

            if (runRewardService != null)
            {
                runRewardService.RewardsChanged += RefreshRewardPanel;
            }

            if (resultPopupView != null)
            {
                resultPopupView.ReviveRequested +=
                    HandleReviveRequested;
                resultPopupView.RestartRequested +=
                    HandleRestartRequested;
            }
        }

        private void OnDisable()
        {
            if (stateMachine != null)
            {
                stateMachine.StateChanged -= HandleStateChanged;
            }

            if (wheelSpinController != null)
            {
                wheelSpinController.SpinStarted -=
                    HandleSpinStarted;

                wheelSpinController.SpinCompleted -=
                    HandleSpinCompleted;

                wheelSpinController.SpinCancelled -=
                    HandleSpinCancelled;
            }

            if (collectButton != null)
            {
                collectButton.onClick.RemoveListener(
                    HandleCollectRequested
                );
            }

            if (runRewardService != null)
            {
                runRewardService.RewardsChanged -= RefreshRewardPanel;
            }

            if (resultPopupView != null)
            {
                resultPopupView.ReviveRequested -=
                    HandleReviveRequested;
                resultPopupView.RestartRequested -=
                    HandleRestartRequested;
            }
        }

        private void Start()
        {
            HideTransientViews();

            if (wheelSpinController != null)
            {
                wheelSpinController.SetInteractionEnabled(
                    interactionPolicy != null && interactionPolicy.CanSpin
                );
            }

            ApplyCurrentZone();
            RefreshRewardPanel();
        }

        private void OnDestroy()
        {
            if (runtimeZoneConfiguration != null)
            {
                Destroy(runtimeZoneConfiguration);
                runtimeZoneConfiguration = null;
            }

            if (runtimeCurrencyWallet != null)
            {
                Destroy(runtimeCurrencyWallet.gameObject);
                runtimeCurrencyWallet = null;
            }
        }

        private void HandleSpinStarted()
        {
            if (interactionPolicy == null || !interactionPolicy.CanSpin)
            {
                return;
            }

            stateMachine.SetState(GameFlowState.Spinning);
            HideWheelResult();
        }

        private void HandleSpinCompleted(
            WheelSliceData selectedSlice
        )
        {
            if (stateMachine == null ||
                stateMachine.CurrentState != GameFlowState.Spinning)
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

                stateMachine.SetState(GameFlowState.ReadyToSpin);
                ApplyCurrentZone();
                return;
            }

            RewardData selectedReward = selectedSlice.Reward;

            int rewardAmount =
                selectedSlice.GetRewardAmount(CurrentZone);

            selectedReward.Resolve(this, rewardAmount);
        }

        private void HandleSpinCancelled()
        {
            if (stateMachine != null &&
                stateMachine.CurrentState == GameFlowState.Spinning)
            {
                stateMachine.SetState(GameFlowState.ReadyToSpin);
                ApplyCurrentZone();
            }
        }

        public void GrantReward(RewardData reward, int amount)
        {
            if (reward == null)
            {
                return;
            }

            bool rewardGranted =
                runFlowService != null &&
                runFlowService.GrantReward(
                    reward,
                    amount
                );

            if (!rewardGranted)
            {
                return;
            }

            if (wheelResultView != null)
            {
                wheelResultView.Show(
                    reward,
                    amount
                );
            }

            Debug.Log(
                $"Zone {CurrentZone} reward: " +
                $"{reward.DisplayName} x{amount}"
            );

            ApplyCurrentZone();
        }

        public void TriggerBomb()
        {
            HandleBomb();
        }

        private void HandleCollectRequested()
        {
            if (resultPopupView == null)
            {
                Debug.LogWarning(
                    "Result popup reference is missing.",
                    this
                );
                return;
            }

            RunCollectionResult result =
                runFlowService != null
                    ? runFlowService.TryCollect()
                    : RunCollectionResult.SettlementFailed;

            switch (result)
            {
                case RunCollectionResult.Success:
                    resultPopupView.ShowCollected(CurrentZone);
                    Debug.Log(
                        $"The player safely collected the rewards in Zone {CurrentZone}."
                    );
                    return;

                case RunCollectionResult.InvalidState:
                    Debug.LogWarning(
                        "Rewards can only be collected while ready to spin.",
                        this
                    );
                    return;

                case RunCollectionResult.MissingZoneDefinition:
                    Debug.LogWarning(
                        "Current zone definition is missing.",
                        this
                    );
                    return;

                case RunCollectionResult.CollectionNotAllowed:
                    Debug.LogWarning(
                        "Rewards can only be collected in a Safe or Super Zone.",
                        this
                    );
                    return;

                default:
                    Debug.LogWarning(
                        "Run rewards could not be settled.",
                        this
                    );
                    return;
            }
        }

        private void HandleBomb()
        {
            RunBombResult result =
                runFlowService != null
                    ? runFlowService.TriggerBomb()
                    : RunBombResult.InvalidState;

            if (result != RunBombResult.Success)
            {
                return;
            }

            HideWheelResult();

            if (resultPopupView != null)
            {
                resultPopupView.ShowBombDecision(
                    CurrentZone,
                    reviveCost,
                    reviveService != null &&
                    reviveService.IsAvailable
                );
            }

            Debug.Log(
                $"Bomb selected in Zone {CurrentZone}. " +
                "Awaiting revive or restart decision."
            );
        }

        private void HandleReviveRequested()
        {
            RunReviveResult result =
                runFlowService != null
                    ? runFlowService.TryRevive()
                    : RunReviveResult.ServiceUnavailable;

            switch (result)
            {
                case RunReviveResult.Success:
                    HideTransientViews();
                    ApplyCurrentZone();

                    Debug.Log(
                        $"Revive succeeded in Zone {CurrentZone} for {reviveCost} Gold."
                    );
                    return;

                case RunReviveResult.InsufficientCurrency:
                    if (resultPopupView != null)
                    {
                        resultPopupView.ShowInsufficientGold(
                            CurrentZone,
                            reviveCost,
                            reviveService != null
                                ? reviveService.CurrentBalance
                                : 0
                        );
                    }

                    Debug.LogWarning(
                        "Not enough Gold to revive.",
                        this
                    );
                    return;

                case RunReviveResult.ServiceUnavailable:
                    Debug.LogWarning(
                        "Revive service is unavailable.",
                        this
                    );
                    return;

                default:
                    return;
            }
        }

        private void HandleRestartRequested()
        {
            if (stateMachine != null &&
                stateMachine.CurrentState == GameFlowState.RunCollected)
            {
                HandleCollectedRestart();
                return;
            }

            RunRestartResult result =
                runFlowService != null
                    ? runFlowService.TryRestart()
                    : RunRestartResult.InvalidState;

            if (result != RunRestartResult.Success)
            {
                return;
            }

            CompleteRestartPresentation(
                "The run restarted from Zone 1."
            );
        }

        private void HandleCollectedRestart()
        {
            RunRestartResult result =
                runFlowService != null
                    ? runFlowService.TryRestart()
                    : RunRestartResult.InvalidState;

            if (result != RunRestartResult.Success)
            {
                return;
            }

            CompleteRestartPresentation(
                "The collected run restarted from Zone 1."
            );
        }

        private void ApplyCurrentZone()
        {
            ZoneDefinition zoneDefinition;

            if (zoneProgressionService == null ||
                !zoneProgressionService.TryGetCurrentDefinition(
                    out zoneDefinition
                ))
            {
                Debug.LogWarning(
                    "Current zone definition is missing.",
                    this
                );
                return;
            }

            WheelData selectedWheel = zoneDefinition.WheelData;

            if (selectedWheel != null &&
                wheelView != null)
            {
                wheelView.SetWheelData(selectedWheel, CurrentZone);
            }

            UpdateZoneUI(zoneDefinition);
        }

        private void UpdateZoneUI(ZoneDefinition zoneDefinition)
        {
            if (zoneValueText != null)
            {
                zoneValueText.text =
                    $"ZONE {CurrentZone}";
            }

            if (zoneTitleText != null)
            {
                switch (zoneDefinition.ZoneType)
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

            RefreshCollectButton(zoneDefinition);
        }

        private void HandleStateChanged(
            GameFlowState previousState,
            GameFlowState currentState
        )
        {
            if (wheelSpinController != null)
            {
                wheelSpinController.SetInteractionEnabled(
                    interactionPolicy != null && interactionPolicy.CanSpin
                );
            }

            ZoneDefinition zoneDefinition;

            if (zoneProgressionService != null &&
                zoneProgressionService.TryGetCurrentDefinition(
                    out zoneDefinition
                ))
            {
                RefreshCollectButton(zoneDefinition);
            }

            if (resultPopupView != null &&
                currentState == GameFlowState.AwaitingBombDecision)
            {
                resultPopupView.SetReviveAvailability(
                    reviveService != null &&
                    reviveService.IsAvailable
                );
            }
        }

        private void RefreshRewardPanel()
        {
            if (rewardPanelView != null)
            {
                rewardPanelView.Refresh(
                    runRewardService != null
                        ? runRewardService.Entries
                        : null
                );
            }
        }

        private void HideTransientViews()
        {
            HideResultPopup();
            HideWheelResult();
        }

        private void HideResultPopup()
        {
            if (resultPopupView != null)
            {
                resultPopupView.Hide();
            }
        }

        private void HideWheelResult()
        {
            if (wheelResultView != null)
            {
                wheelResultView.Hide();
            }
        }

        private void RefreshCollectButton(
            ZoneDefinition zoneDefinition
        )
        {
            if (collectButton != null)
            {
                collectButton.interactable =
                    interactionPolicy != null &&
                    interactionPolicy.CanCollect(zoneDefinition);
            }
        }

        private void CompleteRestartPresentation(
            string logMessage
        )
        {
            HideTransientViews();

            if (wheelSpinController != null)
            {
                wheelSpinController.ResetWheelRotation();
            }

            ApplyCurrentZone();
            Debug.Log(logMessage);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (currencyWallet == null)
            {
                currencyWallet =
                    GetComponent<CurrencyWallet>();
            }
        }
#endif
    }
}
