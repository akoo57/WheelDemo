# Wheel Demo

A Unity-based wheel reward demo developed as a Game Developer assignment.

## Unity Version

Unity 2021.3.45f2

## Platform

- Android
- Landscape orientation
- Tested with 16:9, 20:9, and 4:3 aspect ratios

## Gameplay

The player spins a reward wheel and progresses through zones while accumulating rewards during the current run.

- Normal Zones use the Bronze Wheel.
- Every 5th zone is a Safe Zone and uses the Silver Wheel.
- Every 30th zone is a Super Zone and uses the Golden Wheel.
- Bronze Wheels contain exactly one bomb reward.
- Safe and Super Wheels do not contain bombs.
- Rewards accumulate during the current run.
- Repeated rewards increase the amount of the existing reward entry.
- Reward amounts scale with zone progression through configurable reward progression data.
- The player can collect and safely settle accumulated rewards only in Safe and Super Zones.
- When a bomb is selected, the player can either restart the run or spend Gold to revive.
- Reviving keeps the player in the current zone and preserves the current run rewards.
- Restarting after a bomb clears uncollected run rewards and returns the player to Zone 1.
- Progression continues beyond Zone 30.

## Controls

- **Spin:** Spins the active wheel.
- **Collect:** Secures accumulated rewards in Safe and Super Zones.
- **Revive:** Spends Gold after a bomb to continue from the same zone while keeping current run rewards.
- **Restart:** Starts a new run from Zone 1 after collecting rewards or choosing not to revive after a bomb.

## Wheel Types

### Bronze Wheel

Used in Normal Zones. Contains standard rewards and exactly one bomb reward.

### Silver Wheel

Used in Safe Zones. Contains improved rewards and no bomb rewards.

### Golden Wheel

Used in Super Zones. Contains high-tier rewards and no bomb rewards.

## Currency Revive Bonus

The project includes an additional Gold-based revive flow.

- The player starts with a configurable Gold balance.
- Revive cost is configurable.
- When a bomb is selected, the run pauses while the player chooses between Revive and Restart.
- A successful Revive deducts Gold, keeps accumulated run rewards, and continues from the same zone.
- If the player cannot afford the revive cost, the UI displays the insufficient Gold state.
- Restarting clears uncollected run rewards and resets progression to Zone 1.

## Feedback Revision Summary

This version revises the original implementation based on the technical feedback received.

- **Game flow was separated into smaller responsibilities.** The original `GameFlowController` handled too much of the gameplay flow directly. The revised version uses `GameFlowStateMachine`, `GameInteractionPolicy`, `RunFlowService`, and focused services, while `GameFlowController` mainly coordinates scene events and UI presentation.

- **Explicit game states were added.** The flow is now represented with `ReadyToSpin`, `Spinning`, `AwaitingBombDecision`, and `RunCollected` states instead of relying only on scattered conditions.

- **Zone progression became data-driven.** Zone rules and wheel selection were moved into `ZoneConfiguration`, `ZoneDefinition`, and `ZoneProgressionService`. Safe and Super Zone intervals and their wheel mappings can now be configured from data.

- **Reward behavior was made extensible.** Standard rewards and bomb behavior are handled through `RewardBehavior`, `StandardRewardBehavior`, and `BombRewardBehavior` instead of keeping bomb-specific behavior branches throughout the gameplay code.

- **Reward progression was moved out of hardcoded fallback logic.** Reward amounts now use `RewardProgressionConfiguration`, allowing progression values to be configured through ScriptableObject data.

- **Runtime fallback configuration was removed.** The revised scene explicitly uses its assigned `ZoneConfiguration` and `CurrencyWallet` instead of silently creating fallback objects at runtime.

- **Wheel bomb rules are validated in the Editor.** Bronze expects exactly one hazard, while Silver and Golden expect none.

- **Wheel spin lifecycle was made safer.** Active spin coroutines are tracked and cancelled safely, and each spin keeps a reference to the wheel data it started with to avoid inconsistent results during the animation.

- **Reward UI now reuses existing item views.** `RewardPanelView` keeps and reuses created `RewardItemView` instances instead of rebuilding the reward list every time it refreshes.

- **Repeated reward visual code was reduced.** Shared icon and amount presentation is handled by `RewardVisualUtility`.

- **Wheel reward content stays upright while spinning.** Slice visuals no longer rotate together with the wheel.

- **UI implementation was revised for the requested technical rules.** The project uses the requested Canvas scaling setup, sliced sprites where appropriate, cleaner raycast/maskable settings, code-based button listeners, and responsive layouts verified at 20:9, 16:9, and 4:3.

- **Legacy and duplicated code was cleaned up.** Unused runtime factories, unused APIs/events, old compatibility fallbacks, and duplicated restart handling were removed.

The core gameplay remains the same, but the revised version is more maintainable, configurable, reliable, and easier to extend.

## Architecture

The project separates gameplay data, game-flow logic, reward systems, and presentation responsibilities.

- `GameFlowController`: Coordinates scene events, services, and UI presentation.
- `GameFlowStateMachine`: Tracks the current gameplay state.
- `RunFlowService`: Handles collect, reward, bomb, revive, and restart flow.
- `ZoneConfiguration` / `ZoneProgressionService`: Configure zone rules and manage zone progression.
- `RewardData` / `RewardBehavior`: Define reward data and reward-specific behavior.
- `RewardProgressionConfiguration`: Controls how reward amounts scale with zone progression.
- `RunRewardService` / `RewardSettlementService`: Manage current-run rewards and settle collected rewards.
- `CurrencyWallet` / `ReviveService`: Handle Gold balance and revive spending.
- `WheelData` / `WheelSpinController`: Define wheel content and handle wheel spinning.
- `RewardPanelView` / `RewardVisualUtility`: Reuse reward UI elements and centralize shared reward visuals.

## Responsive UI

- Reference resolution: `1920 x 1080`
- Canvas scaling mode: `Scale With Screen Size`
- Screen match mode: `Expand`
- UI uses anchored layouts and sliced sprites where appropriate.
- Non-interactive graphics avoid unnecessary raycast targets.
- Supported test ratios:
  - `16:9`
  - `20:9`
  - `4:3`
- Android orientation is restricted to landscape.

## Android Build

The installable Android APK is available from the repository's Releases section.

## Screenshots

### 16:9

![16:9 Gameplay](Screenshots/WheelDemo_16x9.png)

### 20:9

![20:9 Gameplay](Screenshots/WheelDemo_20x9.png)

### 4:3

![4:3 Gameplay](Screenshots/WheelDemo_4x3.png)

## Gameplay Video

[Watch the gameplay video on Google Drive](https://drive.google.com/file/d/19BL6LrDqL3vmovxvPIwgZp-95XSOQtck/view?usp=sharing)
