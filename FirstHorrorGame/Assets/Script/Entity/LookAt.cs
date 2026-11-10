using UnityEngine;
using UnityEngine.Animations;
using Sirenix.OdinInspector;

namespace ENZEUN.Runtime
{
    public class LookAt : MonoBehaviour
    {
        [Header("Target & Constraint")]
        public Transform playerTransform;
        public LookAtConstraint lookAtConstraint;

        [Header("Reference (NPC 몸통/Root)")]
        public Transform npcRoot;

        [Header("Settings")]
        [SerializeField] private float lerpSpeed = 5f;
        [SerializeField] private float maxAngle = 70f;

        private float forceWeight = -1f;

        [Header("Debug"), SerializeField, ReadOnly]
        private float angle;

        void Update()
        {
            if (playerTransform == null || lookAtConstraint == null || npcRoot == null) return;

            float targetWeight = 0f;

            // 1. 강제 Weight 모드 체크
            if (forceWeight >= 0f && forceWeight <= 1f)
            {
                targetWeight = forceWeight;
            }
            else
            {
                // 2. 수평 위치 계산 (Y축 높이 제거)
                Vector3 npcPos = new Vector3(npcRoot.position.x, 0, npcRoot.position.z);
                Vector3 playerPos = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);

                Vector3 dirToPlayer = (playerPos - npcPos).normalized;
                Vector3 rootForward = new Vector3(npcRoot.forward.x, 0, npcRoot.forward.z).normalized;

                // 3. 각도 계산
                angle = Vector3.Angle(rootForward, dirToPlayer);

                // 4. Target Weight 설정
                targetWeight = (angle < maxAngle) ? 1f : 0f;
            }

            // 5. 확실한 0/1 도달을 위한 MoveTowards 보정 처리
            float currentWeight = lookAtConstraint.weight;
            float nextWeight = Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime * lerpSpeed);

            // 오차 범위 내 도달 시 완벽하게 값 고정
            if (Mathf.Abs(nextWeight - targetWeight) < 0.001f)
            {
                nextWeight = targetWeight;
            }

            lookAtConstraint.weight = nextWeight;
        }

        [Tooltip("런타임에서 강제로 Weight를 설정할 수 있습니다. -1일 경우 자동 Weight 제어")]
        [Button]
        public void SetForceWeight(float weight)
        {
            if (weight < 0f)
                forceWeight = -1f;
            else
                forceWeight = Mathf.Clamp01(weight);
        }
    }
}