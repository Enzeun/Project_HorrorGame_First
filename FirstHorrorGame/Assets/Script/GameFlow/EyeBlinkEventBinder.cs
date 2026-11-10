using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ENZEUN.Runtime
{

    public class EyeBlinkEventBinder : MonoBehaviour
    {
        [SerializeField, Required, BoxGroup("필수 참조 필드")]
        private EyeBlinkController eyeBlinkController;

        public enum EventType
        {
            OnEyesClose,
            OnEyesOpen,
            Both
        }

        public EventType eventType = EventType.OnEyesClose;

        public UnityEvent OnEyesClosed;
        public UnityEvent OnEyesOpened;

        private bool isBoundOpen;
        private bool isBoundClose;

        public void EyeBlinkStart()
        {
            if (eyeBlinkController == null || isBoundOpen || isBoundClose)
            {
                Debug.Log("eyeBlinkController is null or already bound.");
                return;
            }

            switch (eventType)
            {
                case EventType.OnEyesClose:
                    isBoundClose = true;
                    eyeBlinkController.OnEyesClosed += HandleEyesClosed;
                    break;

                case EventType.OnEyesOpen:
                    isBoundOpen = true;
                    eyeBlinkController.OnEyesOpened += HandleEyesOpened;
                    break;

                case EventType.Both:
                    isBoundClose = true;
                    isBoundOpen = true;
                    eyeBlinkController.OnEyesClosed += HandleEyesClosed;
                    eyeBlinkController.OnEyesOpened += HandleEyesOpened;
                    break;
            }

            eyeBlinkController.BlinkEyes();
        }

        private void HandleEyesClosed()
        {
            eyeBlinkController.OnEyesClosed -= HandleEyesClosed;
            OnEyesClosed?.Invoke();
            isBoundClose = false;
        }

        private void HandleEyesOpened()
        {
            eyeBlinkController.OnEyesOpened -= HandleEyesOpened;
            OnEyesOpened?.Invoke();
            isBoundOpen = false;
        }

        // 연출 도중 파괴되면 컨트롤러의 Action 에 핸들러가 남으므로 해제합니다.
        private void OnDestroy()
        {
            if (eyeBlinkController == null)
                return;

            if (isBoundClose)
            {
                eyeBlinkController.OnEyesClosed -= HandleEyesClosed;
                isBoundClose = false;
            }

            if (isBoundOpen)
            {
                eyeBlinkController.OnEyesOpened -= HandleEyesOpened;
                isBoundOpen = false;
            }
        }
    }

}