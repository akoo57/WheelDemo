using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WheelDemo.Data;

namespace WheelDemo.Wheel
{
    public class WheelSpinController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform wheelAnimator;
        [SerializeField] private WheelView wheelView;
        [SerializeField] private Button spinButton;

        [Header("Spin Settings")]
        [SerializeField, Min(0.5f)] private float spinDuration = 3.5f;
        [SerializeField, Min(1)] private int fullTurnCount = 5;

        private bool isSpinning;
        private bool interactionAllowed = true;
        private Coroutine spinCoroutine;

        public event Action SpinStarted;
        public event Action<WheelSliceData> SpinCompleted;
        public event Action SpinCancelled;
        
        public bool IsSpinning => isSpinning;

        private void OnEnable()
        {
            if (spinButton != null)
            {
                spinButton.onClick.AddListener(HandleSpinButton);
            }
        }

        private void OnDisable()
        {
            CancelActiveSpin(true);

            if (spinButton != null)
            {
                spinButton.onClick.RemoveListener(HandleSpinButton);
            }
        }

        public void SetInteractionEnabled(bool isEnabled)
        {
            interactionAllowed = isEnabled;

            if (spinButton != null)
            {
                spinButton.interactable =
                    interactionAllowed && !isSpinning;
            }
        }

        public void ResetWheelRotation()
        {
            CancelActiveSpin(true);

            if (wheelAnimator != null)
            {
                wheelAnimator.localRotation = Quaternion.identity;
            }

            if (spinButton != null)
            {
                spinButton.interactable = interactionAllowed;
            }
        }

        private void HandleSpinButton()
        {
            if (isSpinning || !interactionAllowed)
            {
                return;
            }

            WheelData wheelData = wheelView.CurrentWheelData;

            if (wheelData == null || wheelData.Slices.Count == 0)
            {
                Debug.LogWarning(
                    "Wheel data is missing or contains no slices.",
                    this
                );

                return;
            }

            int selectedSliceIndex =
                UnityEngine.Random.Range(0, wheelData.Slices.Count);

            SpinStarted?.Invoke();

            spinCoroutine =
                StartCoroutine(SpinRoutine(selectedSliceIndex));
        }

        private IEnumerator SpinRoutine(int selectedSliceIndex)
        {
            isSpinning = true;

            if (spinButton != null)
            {
                spinButton.interactable = false;
            }

            int sliceCount =
                wheelView.CurrentWheelData.Slices.Count;

            float anglePerSlice = 360f / sliceCount;

            float startAngle =
                wheelAnimator.localEulerAngles.z;

            float startNormalizedAngle =
                Mathf.Repeat(startAngle, 360f);

            float selectedSliceAngle =
                selectedSliceIndex * anglePerSlice;

            float angleUntilSelectedSlice =
                Mathf.Repeat(
                    selectedSliceAngle - startNormalizedAngle,
                    360f
                );

            float targetAngle =
                startAngle +
                fullTurnCount * 360f +
                angleUntilSelectedSlice;

            float elapsedTime = 0f;

            while (elapsedTime < spinDuration)
            {
                elapsedTime += Time.deltaTime;

                float normalizedTime =
                    Mathf.Clamp01(elapsedTime / spinDuration);

                float easedTime =
                    EaseInOutCubic(normalizedTime);

                float currentAngle =
                    Mathf.LerpUnclamped(
                        startAngle,
                        targetAngle,
                        easedTime
                    );

                wheelAnimator.localRotation =
                    Quaternion.Euler(0f, 0f, currentAngle);

                yield return null;
            }

            wheelAnimator.localRotation =
                Quaternion.Euler(0f, 0f, targetAngle);

            WheelSliceData selectedSlice =
                wheelView.CurrentWheelData.Slices[selectedSliceIndex];

            isSpinning = false;
            spinCoroutine = null;

            SpinCompleted?.Invoke(selectedSlice);

            if (spinButton != null)
            {
                spinButton.interactable = interactionAllowed;
            }
        }

        private void CancelActiveSpin(bool notifyCancellation)
        {
            if (spinCoroutine == null)
            {
                return;
            }

            StopCoroutine(spinCoroutine);
            spinCoroutine = null;
            isSpinning = false;

            if (spinButton != null)
            {
                spinButton.interactable = interactionAllowed;
            }

            if (notifyCancellation)
            {
                SpinCancelled?.Invoke();
            }
        }

        private static float EaseInOutCubic(float value)
        {
            return value < 0.5f
                ? 4f * value * value * value
                : 1f - Mathf.Pow(-2f * value + 2f, 3f) / 2f;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (wheelAnimator == null)
            {
                Transform animatorTransform =
                    transform.Find("ui_animator_wheel");

                if (animatorTransform != null)
                {
                    wheelAnimator =
                        animatorTransform.GetComponent<RectTransform>();
                }
            }

            if (wheelView == null)
            {
                wheelView = GetComponentInChildren<WheelView>(true);
            }

            if (spinButton == null)
            {
                Transform buttonTransform =
                    transform.Find("ui_button_spin");

                if (buttonTransform != null)
                {
                    spinButton = buttonTransform.GetComponent<Button>();
                }
            }
        }
#endif
    }
}
