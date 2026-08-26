using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UHFPS.Runtime;

namespace ENZEUN.Runtime
{

    public class LockDoorAfterEvent : MonoBehaviour
    {
        [BoxGroup("목표 문")]
        [SerializeField] private DynamicObject doorToLock;

        [BoxGroup("추적 디버깅")]
        [SerializeField, ReadOnly] private bool Started = false;
        [BoxGroup("추적 디버깅")]
        [ShowInInspector, ReadOnly] public bool Finished { get; private set; } = false;

        [BoxGroup("이벤트")]
        public UnityEvent OnFinished;

        [BoxGroup("디버깅"), Button]
        public void LockDoor()
        {
            if (doorToLock == null || Started)
                return;

            Started = true;

            if (doorToLock.IsOpened)
            {
                doorToLock.useEvent2.AddListener(WaitForAnimation);
                doorToLock.InteractStartPlayer(gameObject);
            }
            else
            {
                FinishMethod();
            }
        }

        private void WaitForAnimation()
        {
            doorToLock.useEvent2.RemoveListener(WaitForAnimation);

            FinishMethod();
        }
        private void FinishMethod()
        {
            doorToLock.SetLockedStatus(true);

            Finished = true;

            OnFinished?.Invoke();
        }
    }
}