using UnityEngine;
using UHFPS.Runtime;
using UnityEngine.Events;


namespace ENZEUN.Runtime
{
    public class ObjectiveCompleteRuntimeBinder : MonoBehaviour
    {
        private ObjectiveManager objectiveManager;
        private bool isBound;

        public UnityEvent OnObjectiveComplete;

        void Awake()
        {
            objectiveManager = ObjectiveManager.Instance;

            if (objectiveManager == null)
            {
                Debug.Log("@@@ 중요 @@@ objective Manager 가 할당되지 않았습니다. @@@");
            }
        }

        public void BindOnObjectiveComplete()
        {
            if (objectiveManager == null || isBound)
                return;

            objectiveManager.OnObjectiveComplete += HandleEvent;

            isBound = true;
        }

        private void HandleEvent()
        {
            objectiveManager.OnObjectiveComplete -= HandleEvent;

            isBound = false;

            OnObjectiveComplete?.Invoke();
        }
    }
}