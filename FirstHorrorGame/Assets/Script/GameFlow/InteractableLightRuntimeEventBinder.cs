using UnityEngine;
using UHFPS.Runtime;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace ENZEUN.Runtime
{

    public class InteractableLightRuntimeEventBinder : MonoBehaviour
    {
        [Header("바인딩 할 InteractableLight / 비어있으면 현재 GameObject의 컴포넌트 참조 / 없으면 안됨")]
        [SerializeField]
        private InteractableLight target;

        [SerializeField, BoxGroup("끝날 때 이벤트 Remove 여부")]
        private bool triggerOnce = true;

        private bool isBound = false;

        [Header("바인드 할 이벤트 / 한 종류만 바인딩 하도록 설계됨 / 이는 인스펙터 설정 실수 방지용임.")]
        public UnityEvent OnLightOn;
        public UnityEvent OnLightOff;


        void Awake()
        {
            if (target == null)
            {
                if (!TryGetComponent<InteractableLight>(out target))
                {
                    Debug.Log("@@@ 중요 @@@ Interactable Light이 지정되지 않았고 현재 GameObject에도 Interactable Light 가 없습니다. @@@");
                }
            }
        }

        // 이벤트 바인딩
        public void BindOnLightOn()
        {
            if (target == null || isBound)
                return;

            isBound = true;

            target.OnLightOn.AddListener(HandleOnLightOn);
        }

        public void BindOnLightOff()
        {
            if (target == null || isBound)
                return;

            isBound = true;

            target.OnLightOff.AddListener(HandleOnLightOff);
        }

        private void HandleOnLightOn()
        {
            OnLightOn?.Invoke();

            if (triggerOnce)
            {
                target.OnLightOn.RemoveListener(HandleOnLightOn);
                isBound = false;
            }
        }

        private void HandleOnLightOff()
        {
            OnLightOff?.Invoke();

            if (triggerOnce)
            {
                target.OnLightOff.RemoveListener(HandleOnLightOff);
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

            target.OnLightOn.RemoveListener(HandleOnLightOn);

            target.OnLightOff.RemoveListener(HandleOnLightOff);

            isBound = false;
        }

    }

}