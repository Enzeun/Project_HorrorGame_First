using UnityEngine;
using UHFPS.Runtime;
using UnityEngine.Events;
using Sirenix.OdinInspector;


namespace ENZEUN.Runtime
{

    public class InteractableItemRuntimeEventBinder : MonoBehaviour
    {
        [Header("바인딩 할 Interactable Item / 비어있으면 현재 GameObject의 컴포넌트 참조 / 없으면 안됨")]
        [SerializeField]
        private InteractableItem item;

        [SerializeField, BoxGroup("끝날 때 이벤트 Remove 여부")]
        private bool triggerOnce = true;

        private bool isBound = false;

        [Header("바인드 할 이벤트 / 한 종류만 바인딩 하도록 설계됨 / 이는 인스펙터 설정 실수 방지용임.")]
        public UnityEvent OnTakeEvent;
        public UnityEvent OnExamineStartEvent;
        public UnityEvent OnExamineEndEvent;

        void Awake()
        {
            if (item == null)
            {
                if (!TryGetComponent<InteractableItem>(out item))
                {
                    Debug.Log("@@@ 중요 @@@ item 이 null 입니다. 확인하세요 @@@");
                }
            }
        }

        // 이벤트 바인딩
        public void BindOnTake()
        {
            if (item == null || isBound)
            {
                return;
            }

            isBound = true;

            item.OnTakeEvent.AddListener(HandleOnTake);
        }

        public void BindOnExamineStart()
        {
            if (item == null || isBound)
            {
                return;
            }

            isBound = true;

            item.OnExamineStartEvent.AddListener(HandleOnExamineStart);
        }

        public void BindOnExamineEnd()
        {
            if (item == null || isBound)
            {
                return;
            }

            isBound = true;

            item.OnExamineEndEvent.AddListener(HandleOnExamineEnd);
        }

        // 이벤트 핸들러
        private void HandleOnTake()
        {
            OnTakeEvent?.Invoke();

            if (triggerOnce)
            {
                item.OnTakeEvent.RemoveListener(HandleOnTake);
                isBound = false;
            }
        }

        private void HandleOnExamineStart()
        {
            OnExamineStartEvent?.Invoke();

            if (triggerOnce)
            {
                item.OnExamineStartEvent.RemoveListener(HandleOnExamineStart);
                isBound = false;
            }
        }

        private void HandleOnExamineEnd()
        {
            OnExamineEndEvent?.Invoke();

            if (triggerOnce)
            {
                item.OnExamineEndEvent.RemoveListener(HandleOnExamineEnd);
                isBound = false;
            }

        }

        /// <summary>
        /// 이벤트 바인딩 강제 제거
        /// </summary>
        public void RemoveBoundEvent()
        {
            if (item == null || !isBound)
                return;

            item.OnTakeEvent.RemoveListener(HandleOnTake);

            item.OnExamineStartEvent.RemoveListener(HandleOnExamineStart);

            item.OnExamineEndEvent.RemoveListener(HandleOnExamineEnd);

            isBound = false;
        }
    }
}
