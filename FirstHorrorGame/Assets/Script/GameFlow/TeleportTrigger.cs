using UnityEngine;
using Sirenix.OdinInspector;

namespace ENZEUN.Runtime
{
    public class TeleportTrigger : MonoBehaviour
    {
        [SerializeField, Required, BoxGroup("필수 참조 필드")]
        private TeleportManager teleportManager;

        [SerializeField, Required, BoxGroup("필수 참조 필드")]
        private Transform teleportTarget;

        [SerializeField, Required, BoxGroup("필수 참조 필드")]
        private Transform lookTarget;

        public void TriggerTeleport()
        {
            if (teleportManager == null || teleportTarget == null || lookTarget == null)
                return;

            teleportManager.TriggerTeleport(teleportTarget, lookTarget);
        }
    }
}
