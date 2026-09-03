using UnityEngine;
using UHFPS.Runtime;

namespace ENZEUN.Runtime
{
    public class TeleportManager : MonoBehaviour
    {
        private DragRigidbody rb;
        private LookController lc;

        private void Awake()
        {
            rb = GetComponentInChildren<DragRigidbody>();

            if (rb == null)
            {
                Debug.LogError("DragRigidbody component not found on the player object.");
                return;
            }

            lc = GetComponentInChildren<LookController>();

            if (lc == null)
            {
                Debug.LogError("LookController component not found on the player object.");
                return;
            }
        }

        public void TriggerTeleport(Transform teleportTarget, Transform lookTarget)
        {
            if (rb == null || lc == null || teleportTarget == null || lookTarget == null)
                return;

            GameManager.Instance.FreezePlayer(true, false, true); // 이동 중에는 플레이어를 고정

            rb.enabled = false; // 이동 중에는 DragRigidbody의 물리적 상호작용을 비활성화

            transform.position = teleportTarget.position;

            lc.LerpRotation(lookTarget, 0.2f); // 이동 후 시점 방향을 목표 위치의 방향으로 설정

            rb.enabled = true; // 이동 후 DragRigidbody의 물리적 상호작용을 다시 활성화

            GameManager.Instance.FreezePlayer(false, false, false); // 이동 후 플레이어의 고정을 해제

            Debug.Log($"Player teleported to {teleportTarget.position}");
        }
    }
}