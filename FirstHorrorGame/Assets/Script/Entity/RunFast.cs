using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace ENZEUN.Runtime
{
    public class RunFast : MonoBehaviour
    {
        [Header("Target & Movement Settings")]
        [Tooltip("이동할 목적지 Transform")]
        public Transform targetTransform;

        [Tooltip("지나가는 데 걸리는 시간 (초 단위, 작을수록 휙 지나감)")]
        public float moveDuration = 0.15f;

        [Header("Option")]
        [Tooltip("목적지에 도착한 후 이 오브젝트를 자동으로 숨길지 여부")]
        public bool disableOnArrival = true;

        private bool isMoving = false;

        /// <summary>
        /// 외부 스크립트나 트리거에서 이 함수를 호출하면 이동을 시작합니다.
        /// </summary>
        public void StartRunFast()
        {
            if (!isMoving && targetTransform != null)
            {
                Uni_RunFast(targetTransform.position).Forget();
            }
        }

        /// <summary>
        /// 위치(Vector3)를 직접 전달하여 이동시킬 수도 있습니다.
        /// </summary>
        public void StartRunFastTo(Vector3 targetPosition)
        {
            if (!isMoving)
            {
                Uni_RunFast(targetPosition).Forget();
            }
        }

        private async UniTask Uni_RunFast(Vector3 destination)
        {
            var token = this.GetCancellationTokenOnDestroy();

            isMoving = true;

            await transform.DOMove(destination, moveDuration)
                            .SetEase(Ease.Linear)
                            .ToUniTask(cancellationToken: token);

            isMoving = false;

            // 도착 후 처리 (오브젝트 끄기)
            if (disableOnArrival)
            {
                gameObject.SetActive(false);
            }
        }
    }
}