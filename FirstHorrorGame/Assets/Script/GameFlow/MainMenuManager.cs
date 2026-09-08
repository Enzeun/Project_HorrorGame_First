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

            SceneManager.LoadScene("ApartmentScene");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}