using UnityEngine;

[CreateAssetMenu(
    fileName = "RewardData_New",
    menuName = "Wheel Demo/Reward Data"
)]
public class RewardData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string rewardId;
    [SerializeField] private string displayName;
    [SerializeField] private RewardType rewardType;

    [Header("Visual")]
    [SerializeField] private Sprite icon;
    [SerializeField, Min(0.1f)] private float iconScale = 1f;

    [Header("Value")]
    [SerializeField, Min(0)] private int baseAmount = 1;

    public string RewardId => rewardId;
    public string DisplayName => displayName;
    public RewardType RewardType => rewardType;
    public Sprite Icon => icon;
    public float IconScale => iconScale;
    public int BaseAmount => baseAmount;

    public bool IsBomb => rewardType == RewardType.Bomb;
}