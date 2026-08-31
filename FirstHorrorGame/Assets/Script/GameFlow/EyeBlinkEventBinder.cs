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
                return;

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
    }

}