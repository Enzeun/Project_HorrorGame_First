using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using UHFPS.Runtime;

namespace ENZEUN.Runtime
{
    public class SubtitleTrigger : MonoBehaviour
    {
        [BoxGroup("디버깅 추적 값"), ShowInInspector, ReadOnly]
        private bool isTriggered = false;

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

            if (freezePlayerMove)
            {
                GameManager.Instance.PlayerPresence.FreezeMovement(true);
            }
            if (freezePlayerLook)
            {
                GameManager.Instance.PlayerPresence.FreezeLook(true);
            }
        }

        public async UniTask PlaySubtitleAsync()
        {
            var token = this.GetCancellationTokenOnDestroy();

            foreach (var subtitleData in subtitleDatas)
            {
                if (subtitleData == null || subtitleData.subtitleAsset == null)
                {
                    Debug.LogWarning("Subtitle data or subtitle asset is null. Skipping this subtitle.");
                    continue;
                }

                var subtitleManager = SubtitleManager.Instance;

                if (subtitleManager == null)
                {
                    Debug.LogError("SubtitleManager instance is not found.");
                    continue;
                }

                await subtitleManager.ShowSubtitleAsync(subtitleData.subtitleAsset.localizedSubtitle);

                await UniTask.Delay(System.TimeSpan.FromSeconds(subtitleData.delayToNextSubtitle), cancellationToken: token);
            }
            FinishSubtitles();
        }

        private void FinishSubtitles()
        {
            isTriggered = false;

            OnSubtitlesFinished?.Invoke();

            if (freezePlayerMove)
            {
                GameManager.Instance.PlayerPresence.FreezeMovement(false);
            }
            if (freezePlayerLook)
            {
                GameManager.Instance.PlayerPresence.FreezeLook(false);
            }
        }
    }
}