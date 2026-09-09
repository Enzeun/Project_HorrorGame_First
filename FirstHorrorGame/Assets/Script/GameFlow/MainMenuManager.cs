using UnityEngine;
using Cysharp.Threading.Tasks;
using UHFPS.Runtime;

namespace ENZEUN.Runtime
{
    public class MainMenuManager : MonoBehaviour
    {
        private void Start()
        {
            // 초기화 작업 수행
            // --- [마우스 커서 표시 및 잠금 해제] ---
            Cursor.visible = true;                      // 커서를 화면에 표시
            Cursor.lockState = CursorLockMode.None;     // 커서 잠금 해제 (자유롭게 이동 가능)
                                                        // ----------------------------------------
        }
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