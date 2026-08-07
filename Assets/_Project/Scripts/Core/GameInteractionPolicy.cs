using WheelDemo.Data;

namespace WheelDemo.Core
{
    public sealed class GameInteractionPolicy
    {
        private readonly GameFlowStateMachine stateMachine;

        public bool CanSpin =>
            stateMachine != null &&
            stateMachine.CurrentState == GameFlowState.ReadyToSpin;

        public GameInteractionPolicy(GameFlowStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public bool CanCollect(ZoneDefinition zoneDefinition)
        {
            return zoneDefinition != null &&
                CanSpin &&
                zoneDefinition.AllowsCollection;
        }
    }
}
