using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UHFPS.Input;
using Unity.AppUI.UI;
using System.Threading;


namespace ENZEUN.Runtime
{
    public class SubtitleManager : Singleton<SubtitleManager>
    {
        [BoxGroup("UI 세팅"), SerializeField, Required]
        private CanvasGroup subtitleUICanvasGroup;
        [BoxGroup("UI 세팅"), SerializeField, Required]
        private TextMeshProUGUI subtitleUIText;
        [BoxGroup("UI 세팅"), SerializeField, Required]
        private CanvasGroup continueUICanvasGroup;

        [BoxGroup("UI 세팅"), SerializeField, MinValue(0.1f)]
        private float fadeInDuration = 0.3f;
        [BoxGroup("UI 세팅"), SerializeField, MinValue(0.1f)]
        private float fadeOutDuration = 0.3f;
        [BoxGroup("UI 세팅"), SerializeField, MinValue(0.01f)]
        [Tooltip("한 글자당 소요 시간")]
        private float timePerCharacter = 0.03f;


        public async UniTask ShowSubtitleAsync(string subtitle, bool isPlayerFreezed)
        {
            var token = this.GetCancellationTokenOnDestroy();

            InitUI();

            FadeInSubtitleUI();

            await PlayTextAnimation(subtitle, token);

            if (isPlayerFreezed)
            {
                ShowContinueUI();
                await UniTask.WaitUntil(() => InputManager.ReadButtonOnce("Fire", Controls.FIRE) || InputManager.ReadButtonOnce("Use", Controls.USE), cancellationToken: token);
            }
            else
            {
                await UniTask.Delay(1000);
            }

            FadeOutSubtitleUI();

            await UniTask.Delay(TimeSpan.FromSeconds(fadeOutDuration), cancellationToken: token);
        }

        private void FadeInSubtitleUI()
        {
            DOTween.Kill(subtitleUICanvasGroup);
            subtitleUICanvasGroup.DOFade(1f, fadeInDuration)
                                 .SetEase(Ease.Linear);
        }
        private void FadeOutSubtitleUI()
        {
            DOTween.Kill(subtitleUICanvasGroup);
            subtitleUICanvasGroup.DOFade(0f, fadeOutDuration)
                                 .SetEase(Ease.Linear);
        }

        private async UniTask PlayTextAnimation(string subtitle, CancellationToken token)
        {
            DOTween.Kill(subtitleUIText);

            float duration = subtitle.Length * timePerCharacter;

            await subtitleUIText.DOText(subtitle, duration)
                                .SetEase(Ease.Linear)
                                .ToUniTask(cancellationToken: token);
        }

        private void ShowContinueUI()
        {
            DOTween.Kill(continueUICanvasGroup);
            continueUICanvasGroup.DOFade(1, fadeInDuration)
                                 .SetEase(Ease.Linear);
        }

        private void InitUI()
        {
            subtitleUICanvasGroup.alpha = 0;
            continueUICanvasGroup.alpha = 0;
            subtitleUIText.text = string.Empty;
        }
    }
}