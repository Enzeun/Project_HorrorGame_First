using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using UHFPS.Runtime;
using System.Threading;

namespace ENZEUN.Runtime
{
    public class SubtitleTrigger : MonoBehaviour
    {
        [BoxGroup("디버깅 추적 값"), ShowInInspector, ReadOnly]
        private bool isTriggered = false;
        [BoxGroup("디버깅 추적 값"), ShowInInspector, ReadOnly]
        private bool isPlayerFreezed = false;

        [SerializeField, BoxGroup("자막 트리거 세팅"), MinValue(0f), Tooltip("자막 시작 전 잠시 대기 시간 / 0 이면 연속해서 Input을 관리하는 Event 가 있을 경우, 중첩되어 제대로 동작이 안될 수 있음 주의")]
        private float delayBeforeSubtitles = 0.1f;
        [SerializeField, BoxGroup("자막 트리거 세팅"), MinValue(0f), Tooltip("자막 종료 후 잠시 대기 시간 / 0 이면 연속해서 Input을 관리하는 Event 가 있을 경우, 중첩되어 제대로 동작이 안될 수 있음 주의")]
        private float delayAfterSubtitles = 0.1f;
        [SerializeField, BoxGroup("자막 트리거 세팅")]
        private bool freezePlayerMove = false;
        [SerializeField, BoxGroup("자막 트리거 세팅")]
        private bool freezePlayerLook = false;
        [SerializeField, BoxGroup("자막 트리거 세팅")]
        private List<SubtitleData> subtitleDatas;

        public UnityEvent OnSubtitlesFinished;

        [System.Serializable]
        public class SubtitleData
        {
            public SubtitleAsset subtitleAsset;
            [MinValue(0f)]
            public float delayToNextSubtitle = 0.5f;
        }

        public void TriggerSubtitles()
        {
            // 중복실행 방지
            if (isTriggered)
                return;

            isTriggered = true;

            PlaySubtitleAsync().Forget();
        }

        public async UniTask PlaySubtitleAsync()
        {
            var token = this.GetCancellationTokenOnDestroy();

            // 연속해서 Player Input을 관리하는 Event 가 있을 경우, 중첩되어 제대로 동작이 안되기에, 잠시 대기 후 자막을 시작하도록 함
            await UniTask.Delay(System.TimeSpan.FromSeconds(delayBeforeSubtitles), cancellationToken: token);

            if (freezePlayerMove)
            {
                isPlayerFreezed = true;
                GameManager.Instance.LockInput(true);
                GameManager.Instance.PlayerPresence.FreezeMovement(true);
            }

            if (freezePlayerLook)
            {
                GameManager.Instance.PlayerPresence.FreezeLook(true);
            }

            var subtitleManager = SubtitleManager.Instance;

            if (subtitleManager == null)
            {
                Debug.LogError("SubtitleManager instance is not found.");
                return;
            }

            foreach (var subtitleData in subtitleDatas)
            {
                if (subtitleData == null || subtitleData.subtitleAsset == null)
                {
                    Debug.LogWarning("Subtitle data or subtitle asset is null. Skipping this subtitle.");
                    continue;
                }

                await subtitleManager.ShowSubtitleAsync(subtitleData.subtitleAsset.localizedSubtitle, subtitleData.subtitleAsset.duration, isPlayerFreezed);

                await UniTask.Delay(System.TimeSpan.FromSeconds(subtitleData.delayToNextSubtitle), cancellationToken: token);
            }
            await FinishSubtitles(token);
        }

        private async UniTask FinishSubtitles(CancellationToken token)
        {
            if (freezePlayerMove)
            {
                GameManager.Instance.LockInput(false);
                GameManager.Instance.PlayerPresence.FreezeMovement(false);
                isPlayerFreezed = false;
            }
            if (freezePlayerLook)
            {
                GameManager.Instance.PlayerPresence.FreezeLook(false);
            }

            await UniTask.Delay(System.TimeSpan.FromSeconds(delayAfterSubtitles), cancellationToken: token); // 잠시 대기 후 종료

            isTriggered = false;

            OnSubtitlesFinished?.Invoke();
        }
    }
}