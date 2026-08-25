using UnityEngine;
using UHFPS.Rendering;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;
using System;

namespace UHFPS.Runtime
{
    public class EyeBlinkController : MonoBehaviour
    {
        [BoxGroup("값 추적"), SerializeField, ReadOnly]
        private EyeBlink eyeBlink;
        [BoxGroup("값 추적"), SerializeField, ReadOnly]
        private float eyesTime = 0f;
        [BoxGroup("값 추적"), SerializeField, ReadOnly]
        private bool isBlinking = false;
        [BoxGroup("값 추적"), SerializeField, ReadOnly]
        private bool isClosed = false;

        [BoxGroup("값 설정"), SerializeField]
        private Volume EyeBlinkPPVolume;
        [BoxGroup("값 설정")]
        public float CloseEyesSpeed = 2f;
        [BoxGroup("값 설정")]
        public float CloseEyesDuration = 1f;
        [BoxGroup("값 설정")]
        public float OpenEyesSpeed = 2f;

        [BoxGroup("이벤트")]
        public Action OnEyesClosed;
        [BoxGroup("이벤트")]
        public Action OnEyesOpened;


        private void Awake()
        {
            EyeBlinkPPVolume.profile.TryGet(out eyeBlink);

            enabled = false;
        }

        private void Update()
        {
            if (isClosed)
            {
                eyesTime += Time.deltaTime;

                if (eyesTime >= CloseEyesDuration)
                {
                    OpenEyes();
                }
            }

            else
            {
                CloseEyes();
            }
        }

        private void CloseEyes()
        {
            float blinkValue = eyeBlink.Blink.value;

            if (blinkValue >= 1)
            {
                isClosed = true;
                OnEyesClosed?.Invoke();
            }
            else
            {
                eyeBlink.Blink.value = Mathf.MoveTowards(blinkValue, 1f, Time.deltaTime * CloseEyesSpeed);
            }
        }

        private void OpenEyes()
        {
            float blinkValue = eyeBlink.Blink.value;

            if (blinkValue <= 0)
            {
                isClosed = false;

                isBlinking = false;

                eyesTime = 0f;

                EyeBlinkPPVolume.weight = 0f;

                enabled = false;

                OnEyesOpened?.Invoke();
            }
            else
            {
                eyeBlink.Blink.value = Mathf.MoveTowards(blinkValue, 0f, Time.deltaTime * OpenEyesSpeed);
            }
        }

        [Button, BoxGroup("눈깜빡이기 테스트")]
        public void BlinkEyes()
        {
            enabled = true;

            if (isBlinking) return;

            isBlinking = true;

            EyeBlinkPPVolume.weight = 1f;

            eyesTime = 0f;
        }

    }
}