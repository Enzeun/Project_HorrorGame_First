using Sirenix.OdinInspector;
using UHFPS.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace ENZEUN.Runtime
{

    public class DynamicObjectEventBinder : MonoBehaviour
    {
        [SerializeField, BoxGroup("바인딩 할 DynamicObject"), Required]
        private DynamicObject target;

        [SerializeField, BoxGroup("끝날 때 이벤트 Remove 여부")]
        private bool triggerOnce = true;

        private bool isBound = false;

        [Header("Open 또는 Close 바인딩을 둘 중 하나만 할 수 있도록 설계. 이는 실수 방지용임.")]
        [BoxGroup("Open 또는 Close 둘 중 하나만 이벤트를 등록하세요")]
        public UnityEvent OnOpen;
        [BoxGroup("Open 또는 Close 둘 중 하나만 이벤트를 등록하세요")]
        public UnityEvent OnClose;

        public void BindOpen()
        {
            if (target == null || isBound)
                return;

            isBound = true;

            target.useEvent1.AddListener(HandleOpen);
        }

        public void BindClose()
        {
            if (target == null || isBound)
                return;

            isBound = true;

            target.useEvent2.AddListener(HandleClose);
        }
        private void HandleOpen()
        {
            OnOpen?.Invoke();

            if (triggerOnce)
            {
                target.useEvent1.RemoveListener(HandleOpen);
                isBound = false;
            }
        }
        private void HandleClose()
        {
            OnClose?.Invoke();

            if (triggerOnce)
            {
                target.useEvent2.RemoveListener(HandleClose);
                isBound = false;
            }
        }

        public void RemoveBoundEvent()
        {
            if (target == null || !isBound)
                return;

            target.useEvent1.RemoveListener(HandleOpen);

            target.useEvent2.RemoveListener(HandleClose);

            isBound = false;
        }
    }
}