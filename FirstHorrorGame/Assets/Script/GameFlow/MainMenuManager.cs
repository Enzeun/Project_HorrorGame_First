using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace ENZEUN.Runtime
{
    public class MainMenuManager : MonoBehaviour
    {
        public void StartGame(int delay)
        {
            StartGameWithDelay(delay).Forget();
        }

        private async UniTask StartGameWithDelay(int delay)
        {
            await UniTask.Delay(delay * 1000);

            LoadingSceneManager.LoadScene("ApartmentScene");
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            // 유니티 에디터 플레이 모드 종료
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // 실제 빌드된 게임 앱 종료 (PC, Mobile 등)
            Application.Quit();
#endif
        }
    }
}