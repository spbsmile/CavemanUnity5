using System.Collections;
using Caveman.DevSetting;
using Caveman.UI.Common;
using Caveman.Utils;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Caveman.UI.Menu
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup tooltipNickname;
        [SerializeField] private InputField inputNickname;
        [SerializeField] private Text metaServerIp;
       
        [SerializeField] private AudioSource audioMusic;
       
        public void Start()
        {
            Screen.orientation = ScreenOrientation.Landscape;

            // temp for developer
            metaServerIp.text += PlayerPrefs.HasKey(DevSettings.KeyIpServer)
                ? PlayerPrefs.GetString(DevSettings.KeyIpServer)
                : DevSettings.IP_SERVER;
        }

        [UsedImplicitly]
        public void LoadSingleGame()
        {
            if (string.IsNullOrEmpty(inputNickname.text))
            {
                ShowNoNameAlert();
            }
            else
            {
                StartCoroutine(LoadLevelWithAudio());
            }
        }

        private IEnumerator LoadLevelWithAudio()
        {
            audioMusic.Play();
            yield return new WaitForSeconds(0.3f);
            LoadingScreen.instance.ProgressTo(1);
        }

        [UsedImplicitly]
        public void LoadMultiplayGame()
        {
            if (string.IsNullOrEmpty(inputNickname.text))
            {
                StartCoroutine(FadeOutTooltip());
            }
            else
            {
                LoadingScreen.instance.ProgressTo(4);
            }
        }

        [UsedImplicitly]
        public void LoadObserverGame()
        {
            LoadingScreen.instance.ProgressTo(7);
        }

        [UsedImplicitly]
        public void LoadMenu()
        {
            SceneManager.LoadScene(0);
        }

        [UsedImplicitly]
        public void Quit()
        {
            Application.Quit();
        }

        [UsedImplicitly]
        public void Settings()
        {
            SceneManager.LoadScene(2);
        }

        [UsedImplicitly]
        public void LoadProfile()
        {
            SceneManager.LoadScene(6);
        }

        private void ShowNoNameAlert()
        {
            StartCoroutine(FadeOutTooltip());
        }

        private IEnumerator FadeOutTooltip()
        {
            tooltipNickname.alpha = 1;
            yield return new WaitForSeconds(2f);
            StartCoroutine(tooltipNickname.FadeOut());
        }
    }
}
