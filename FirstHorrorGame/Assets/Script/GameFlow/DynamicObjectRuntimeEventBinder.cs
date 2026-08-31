using Sirenix.OdinInspector;
using UHFPS.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace ENZEUN.Runtime
{
    public class DynamicObjectEventBinder : MonoBehaviour
    {
        [Header("바인딩 할 DynamicObject / 비어있으면 현재 GameObject의 컴포넌트 참조 / 없으면 안됨")]
        [SerializeField]
        private DynamicObject target;

        [SerializeField, BoxGroup("끝날 때 이벤트 Remove 여부")]
        private bool triggerOnce = true;

        private bool isBound = false;

        [Header("Open 또는 Close 바인딩을 둘 중 하나만 할 수 있도록 설계. 이는 실수 방지용임.")]
        [BoxGroup("Open 또는 Close 둘 중 하나만 이벤트를 등록하세요")]
        public UnityEvent OnOpen;
        [BoxGroup("Open 또는 Close 둘 중 하나만 이벤트를 등록하세요")]
        public UnityEvent OnClose;

        void Awake()
        {
            if (target == null)
            {
                if (!TryGetComponent<DynamicObject>(out target))
                {
                    Debug.Log("@@@ 중요 @@@ Dynamic Object 가 지정되지 않았고 현재 GameObject에도 Dynamic Object 가 없습니다. @@@");
                }
            }
        }

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

        /// <summary>
        /// 이벤트 바인딩 강제 제거
        /// </summary>
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