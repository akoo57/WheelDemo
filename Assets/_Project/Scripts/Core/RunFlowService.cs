using WheelDemo.Data;
using WheelDemo.Rewards;

namespace WheelDemo.Core
{
    public sealed class RunFlowService
    {
        private readonly GameFlowStateMachine stateMachine;
        private readonly GameInteractionPolicy interactionPolicy;
        private readonly ZoneProgressionService zoneProgressionService;
        private readonly RewardSettlementService rewardSettlementService;
        private readonly RunRewardService runRewardService;
        private readonly ReviveService reviveService;

        public RunFlowService(
            GameFlowStateMachine stateMachine,
            GameInteractionPolicy interactionPolicy,
            ZoneProgressionService zoneProgressionService,
            RewardSettlementService rewardSettlementService,
            RunRewardService runRewardService,
            ReviveService reviveService
        )
        {
            this.stateMachine = stateMachine;
            this.interactionPolicy = interactionPolicy;
            this.zoneProgressionService = zoneProgressionService;
            this.rewardSettlementService = rewardSettlementService;
            this.runRewardService = runRewardService;
            this.reviveService = reviveService;
        }

        public RunCollectionResult TryCollect()
        {
            if (stateMachine == null ||
                stateMachine.CurrentState != GameFlowState.ReadyToSpin)
            {
                return RunCollectionResult.InvalidState;
            }

            ZoneDefinition zoneDefinition;

            if (zoneProgressionService == null ||
                !zoneProgressionService.TryGetCurrentDefinition(
                    out zoneDefinition
                ))
            {
                return RunCollectionResult.MissingZoneDefinition;
            }

            if (interactionPolicy == null ||
                !interactionPolicy.CanCollect(zoneDefinition))
            {
                return RunCollectionResult.CollectionNotAllowed;
            }

            if (rewardSettlementService == null ||
                runRewardService == null ||
                !rewardSettlementService.TrySettle(runRewardService.Entries))
            {
                return RunCollectionResult.SettlementFailed;
            }

            runRewardService.Clear();
            stateMachine.SetState(GameFlowState.RunCollected);
            return RunCollectionResult.Success;
        }

        public bool GrantReward(
            RewardData reward,
            int amount
        )
        {
            if (stateMachine == null ||
                stateMachine.CurrentState != GameFlowState.Spinning ||
                runRewardService == null ||
                zoneProgressionService == null ||
                reward == null)
            {
                return false;
            }

            runRewardService.AddReward(
                reward,
                amount
            );

            zoneProgressionService.Advance();
            stateMachine.SetState(GameFlowState.ReadyToSpin);
            return true;
        }

        public RunBombResult TriggerBomb()
        {
            if (stateMachine == null ||
                stateMachine.CurrentState != GameFlowState.Spinning)
            {
                return RunBombResult.InvalidState;
            }

            stateMachine.SetState(GameFlowState.AwaitingBombDecision);
            return RunBombResult.Success;
        }

        public RunReviveResult TryRevive()
        {
            if (stateMachine == null ||
                stateMachine.CurrentState != GameFlowState.AwaitingBombDecision)
            {
                return RunReviveResult.InvalidState;
            }

            if (reviveService == null)
            {
                return RunReviveResult.ServiceUnavailable;
            }

            if (!reviveService.IsAvailable)
            {
                return RunReviveResult.InsufficientCurrency;
            }

            if (!reviveService.TryRevive())
            {
                return RunReviveResult.InsufficientCurrency;
            }

            stateMachine.SetState(GameFlowState.ReadyToSpin);
            return RunReviveResult.Success;
        }

        public RunRestartResult TryRestart()
        {
            if (stateMachine == null ||
                zoneProgressionService == null ||
                runRewardService == null)
            {
                return RunRestartResult.InvalidState;
            }

            if (stateMachine.CurrentState != GameFlowState.AwaitingBombDecision &&
                stateMachine.CurrentState != GameFlowState.RunCollected)
            {
                return RunRestartResult.InvalidState;
            }

            runRewardService.Clear();
            zoneProgressionService.Reset();
            stateMachine.SetState(GameFlowState.ReadyToSpin);
            return RunRestartResult.Success;
        }
    }
}
