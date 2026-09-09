using UnityEngine;
using UHFPS.Runtime;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace ENZEUN.Runtime
{
    public class EndingScript : MonoBehaviour
    {

        public AudioSource audioSource;
        public AudioClip audioClip;
        public CanvasGroup canvasGroup1;
        public CanvasGroup canvasGroup2;

        [Button("Go To Ending Scene")]
        public void GoToEndingScene()
        {
            gameObject.SetActive(true);

            GameManager.Instance.FreezePlayer(true);

            GoToEndingSceneAsync().Forget();
        }

        private async UniTask GoToEndingSceneAsync()
        {
            var token = this.GetCancellationTokenOnDestroy();

            await UniTask.Delay(2000, cancellationToken: token); // 2초 대기

            if (audioSource != null && audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("AudioSource or AudioClip is not assigned.");
            }

            await UniTask.Delay(5000, cancellationToken: token); // 5초 대기

            //audioSource.Stop();

            await canvasGroup1.DOFade(1f, 1f)
                              .SetEase(Ease.Linear)
                              .ToUniTask(cancellationToken: token);

            await UniTask.Delay(2000, cancellationToken: token); // 2초 대기

            await canvasGroup1.DOFade(0f, 1f)
                              .SetEase(Ease.Linear)
                              .ToUniTask(cancellationToken: token);

            await UniTask.Delay(2000, cancellationToken: token); // 2초 대기

            await canvasGroup2.DOFade(1f, 1f)
                              .SetEase(Ease.Linear)
                              .ToUniTask(cancellationToken: token);

            await UniTask.Delay(2000, cancellationToken: token); // 2초 대기

            await canvasGroup2.DOFade(0f, 1f)
                              .SetEase(Ease.Linear)
                              .ToUniTask(cancellationToken: token);

            LoadingSceneManager.LoadScene("MainMenu");
        }
    }
}