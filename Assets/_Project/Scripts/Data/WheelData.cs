using System.Collections.Generic;
using UnityEngine;

namespace WheelDemo.Data
{
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (slices == null || slices.Count == 0)
            {
                return;
            }

            int hazardCount = 0;

            for (int i = 0; i < slices.Count; i++)
            {
                WheelSliceData slice = slices[i];

                if (slice == null || slice.Reward == null)
                {
                    continue;
                }

                if (slice.Reward.IsHazard)
                {
                    hazardCount++;
                }
            }

            int expectedHazardCount;

            if (!TryGetExpectedHazardCount(out expectedHazardCount))
            {
                return;
            }

            if (hazardCount != expectedHazardCount)
            {
                Debug.LogWarning(
                    $"WheelData '{name}' expects {expectedHazardCount} hazard reward(s), but currently has {hazardCount}.",
                    this
                );
            }
        }

        private bool TryGetExpectedHazardCount(out int expectedHazardCount)
        {
            expectedHazardCount = 0;

            switch (wheelId)
            {
                case "bronze":
                    expectedHazardCount = 1;
                    return true;

                case "silver":
                case "golden":
                    expectedHazardCount = 0;
                    return true;

                default:
                    return false;
            }
        }
#endif
    }
}
