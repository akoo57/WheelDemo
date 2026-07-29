using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WheelData_New",
    menuName = "Wheel Demo/Wheel Data"
)]
public class WheelData : ScriptableObject
{
    [SerializeField] private string wheelId;
    [SerializeField] private Sprite wheelBaseSprite;
    [SerializeField] private Sprite indicatorSprite;
    [SerializeField] private List<WheelSliceData> slices = new List<WheelSliceData>();

    public string WheelId => wheelId;
    public Sprite WheelBaseSprite => wheelBaseSprite;
    public Sprite IndicatorSprite => indicatorSprite;
    public IReadOnlyList<WheelSliceData> Slices => slices;

    public bool HasValidSliceCount => slices != null && slices.Count > 0;
}