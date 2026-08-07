using System;
using UnityEngine;
using UnityEngine.UI;
using WheelDemo.Data;

namespace WheelDemo.Wheel
{
    public class WheelView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private WheelData wheelData;

        [Header("Main Visuals")]
        [SerializeField] private Image wheelBaseImage;
        [SerializeField] private Image indicatorImage;

        [Header("Slices")]
        [SerializeField] private WheelSliceView[] sliceViews;

        public WheelData CurrentWheelData => wheelData;

        public void SetWheelData(
            WheelData newWheelData,
            int zoneNumber
        )
        {
            if (newWheelData == null)
            {
                Debug.LogWarning("New wheel data is null.", this);
                return;
            }

            wheelData = newWheelData;
            RefreshWheel(zoneNumber);
        }

        private void Awake()
        {
            RefreshWheel();
        }

        [ContextMenu("Refresh Wheel")]
        public void RefreshWheel()
        {
            if (wheelData == null)
            {
                return;
            }

            ApplyMainVisuals();
            ApplySlices();
        }

        public void RefreshWheel(int zoneNumber)
        {
            if (wheelData == null)
            {
                return;
            }

            ApplyMainVisuals();
            ApplySlices(zoneNumber);
        }

        private void ApplyMainVisuals()
        {
            if (wheelBaseImage != null)
            {
                wheelBaseImage.sprite = wheelData.WheelBaseSprite;
                wheelBaseImage.enabled = wheelData.WheelBaseSprite != null;
            }

            if (indicatorImage != null)
            {
                indicatorImage.sprite = wheelData.IndicatorSprite;
                indicatorImage.enabled = wheelData.IndicatorSprite != null;
            }
        }

        private void ApplySlices()
        {
            ApplySlicesInternal(false, 0);
        }

        private void ApplySlices(int zoneNumber)
        {
            ApplySlicesInternal(true, zoneNumber);
        }

        private void ApplySlicesInternal(
            bool showAmounts,
            int zoneNumber
        )
        {
            if (sliceViews == null)
            {
                return;
            }

            int dataCount = wheelData.Slices.Count;

            for (int i = 0; i < sliceViews.Length; i++)
            {
                WheelSliceView sliceView = sliceViews[i];

                if (sliceView == null)
                {
                    continue;
                }

                bool hasSliceData = i < dataCount;
                sliceView.gameObject.SetActive(hasSliceData);

                if (hasSliceData)
                {
                    if (showAmounts)
                    {
                        sliceView.Setup(
                            wheelData.Slices[i],
                            zoneNumber
                        );
                    }
                    else
                    {
                        sliceView.Setup(wheelData.Slices[i]);
                    }
                }
            }

            if (dataCount != sliceViews.Length)
            {
                Debug.LogWarning(
                    $"Wheel data contains {dataCount} slices, " +
                    $"but the scene contains {sliceViews.Length} slice views.",
                    this
                );
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            FindReferences();

            if (!Application.isPlaying)
            {
                RefreshWheel();
            }
        }

        private void FindReferences()
        {
            if (wheelBaseImage == null)
            {
                Transform wheelBaseTransform =
                    transform.Find("ui_animator_wheel/ui_image_wheel_base");

                if (wheelBaseTransform != null)
                {
                    wheelBaseImage =
                        wheelBaseTransform.GetComponent<Image>();
                }
            }

            if (indicatorImage == null)
            {
                Transform indicatorTransform =
                    transform.Find("ui_image_wheel_indicator");

                if (indicatorTransform != null)
                {
                    indicatorImage =
                        indicatorTransform.GetComponent<Image>();
                }
            }

            if (sliceViews == null || sliceViews.Length == 0)
            {
                sliceViews =
                    GetComponentsInChildren<WheelSliceView>(true);

                Array.Sort(
                    sliceViews,
                    (first, second) =>
                        string.CompareOrdinal(first.name, second.name)
                );
            }
        }
#endif
    }
}
