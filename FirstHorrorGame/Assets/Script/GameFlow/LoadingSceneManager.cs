using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    private static string nextSceneName;

    public Slider loadingSlider; // 로딩 UI의 슬라이더
    public float sliderFillSpeed = 0.8f; // 슬라이더가 차오르는 속도 (숫자가 작을수록 천천히 차오름)

    /// <summary>
    /// 다른 씬에서 로딩 씬을 거쳐 목표 씬으로 이동할 때 호출합니다.
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        nextSceneName = sceneName;
        SceneManager.LoadScene("MyLoadingScene");
    }

    private void Start()
    {
        // 테스트용: 다른 씬을 안 거치고 로딩씬만 직접 플레이했을 때 예외 처리
        if (string.IsNullOrEmpty(nextSceneName))
        {
            nextSceneName = "ApartmentScene"; // 실제 테스트할 씬 이름 작성
        }

        if (loadingSlider != null)
        {
            loadingSlider.value = 0f;
        }

        LoadNextSceneAsync().Forget();
    }

    private async UniTaskVoid LoadNextSceneAsync()
    {
        // 1. 비동기 씬 로드 시작 (자동 전환 방지)
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;

        float targetProgress = 0f;

        // 2. 비동기 씬 로딩 상태 확인용 progress 생성
        var progress = Progress.Create<float>(p =>
        {
            // AsyncOperation의 progress는 0~0.9 범위이므로 0~1.0 범위로 정규화
            targetProgress = Mathf.Clamp01(p / 0.9f);
        });

        // 3. 씬 데이터 백그라운드 로딩 대기
        var loadTask = op.ToUniTask(progress: progress);

        // 4. [슬라이더 부드럽게 채우기] 
        // 씬 로드가 아직 안 끝났거나, 슬라이더가 100%(1.0)에 도달하지 않았다면 매 프레임 부드럽게 증가
        while (targetProgress < 1.0f || loadingSlider.value < 0.999f)
        {
            // 실제 로딩 값(targetProgress)을 향해 슬라이더를 부드럽게 이동
            loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, targetProgress, Time.deltaTime * sliderFillSpeed);

            //Debug.Log($"LoadingSceneManager: {nextSceneName} 씬 로딩 진행률: {loadingSlider.value * 100f}%");
            // 다음 프레임까지 대기
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        //Debug.Log($"LoadingSceneManager: {nextSceneName} 씬 로딩 완료, 씬 전환 승인");

        // 5. 슬라이더가 100% 채워진 후 '1초(1000ms) 대기'
        await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());

        // 6. 실제 씬 전환 승인
        op.allowSceneActivation = true;
    }
}