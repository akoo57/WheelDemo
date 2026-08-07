using UnityEngine;
using System;

namespace WheelDemo.Core
{
    public enum GameFlowState
    {
        ReadyToSpin,
        Spinning,
        AwaitingBombDecision,
        RunCollected
    }

    public sealed class GameFlowStateMachine
    {
        public event Action<GameFlowState, GameFlowState> StateChanged;

        public GameFlowState CurrentState { get; private set; }

        public GameFlowStateMachine(
            GameFlowState initialState = GameFlowState.ReadyToSpin
        )
        {
            CurrentState = initialState;
        }

        public void SetState(GameFlowState newState)
        {
            if (CurrentState == newState)
            {
                return;
            }

            GameFlowState previousState = CurrentState;
            CurrentState = newState;

            Debug.Log($"Game Flow State: {previousState} -> {newState}");

            StateChanged?.Invoke(previousState, newState);
        }
    }
}
