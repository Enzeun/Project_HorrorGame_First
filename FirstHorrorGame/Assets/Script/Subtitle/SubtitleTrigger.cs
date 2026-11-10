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

            // 입력을 잠그기 전에 매니저를 먼저 확인합니다. (매니저가 없어 return 할 때 입력이 잠긴 채로 남지 않도록)
            var subtitleManager = SubtitleManager.Instance;

            if (subtitleManager == null)
            {
                Debug.LogError("[SubtitleTrigger] : SubtitleManager 를 찾을 수 없어 자막을 재생하지 않습니다.");
                isTriggered = false;
                return;
            }

            SetPlayerFreeze(true);

            try
            {
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
            }
            finally
            {
                // 정상 종료든 취소(파괴)든 잠근 입력은 반드시 풀어 줍니다.
                SetPlayerFreeze(false);
            }

            await UniTask.Delay(System.TimeSpan.FromSeconds(delayAfterSubtitles), cancellationToken: token); // 잠시 대기 후 종료

            isTriggered = false;

            OnSubtitlesFinished?.Invoke();
        }

        /// <summary>
        /// 설정에 따라 플레이어의 이동/시점 입력을 잠그거나 풉니다.
        /// </summary>
        private void SetPlayerFreeze(bool freeze)
        {
            var gameManager = GameManager.Instance;

            // 씬 종료 중이면 GameManager 가 먼저 파괴되었을 수 있습니다.
            if (gameManager == null)
                return;

            if (freezePlayerMove)
            {
                gameManager.LockInput(freeze);
                gameManager.PlayerPresence.FreezeMovement(freeze);
                isPlayerFreezed = freeze;
            }

            if (freezePlayerLook)
            {
                gameManager.PlayerPresence.FreezeLook(freeze);
            }
        }
    }
}