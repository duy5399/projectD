using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoadingScene : MonoBehaviour
{
    public static AsyncLoadingScene instance { get; private set; }

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this );
        }
        else
        {
            instance = this;
        }
        loadingNewScene.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(false);
    }

    [SerializeField] private Transform loadingNewScene;
    [SerializeField] private Transform loadingScreen;
    [SerializeField] private Slider sliderLoadingProcess;
    [SerializeField] private TextMeshProUGUI txtLoadingProcess;

    public void LoadNewScene(int sceneName)
    {
        loadingNewScene.gameObject.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(int sceneName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOperation.isDone)
        {
            sliderLoadingProcess.value = 1 - loadOperation.progress;
            txtLoadingProcess.text = "Đang tải, xin chờ: " + string.Format("{0:0}", loadOperation.progress * 100) + "%";
            yield return null;
        }
        loadingNewScene.gameObject.SetActive(false);

        //PhotonNetwork.LoadLevel(sceneName);

        //while (PhotonNetwork.LevelLoadingProgress < 1)
        //{
        //    sliderLoadingProcess.value = 1 - PhotonNetwork.LevelLoadingProgress;
        //    txtLoadingProcess.text = "Đang tải, xin chờ: " + string.Format("{0:0}", PhotonNetwork.LevelLoadingProgress * 100) + "%";
        //    yield return null;
        //}
        //loadingNewScene.gameObject.SetActive(false);

        //LauncherHomepage.instance.ConnectToPhotonPUN();
        //while (!PhotonNetwork.InRoom)
        //{
        //    sliderLoadingProcess.value = 1 - PhotonNetwork.LevelLoadingProgress;
        //    txtLoadingProcess.text = "Đang tải, xin chờ: " + string.Format("{0:0}", PhotonNetwork.LevelLoadingProgress * 100) + "%";
        //    Debug.Log("!PhotonNetwork.InRoom");
        //    yield return null;
        //}
        //loadingNewScene.gameObject.SetActive(false);
    }

    public void LoadingScreen()
    {
        loadingScreen.gameObject.SetActive(!loadingScreen.gameObject.activeSelf);
    }

    public void LoadingScreen(bool boolean)
    {
        loadingScreen.gameObject.SetActive(boolean);
    }
}
