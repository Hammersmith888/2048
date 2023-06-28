using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingSceneScript : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Image LoadingFill;
    public int indexSceneToLoad;

    private void Start()
    {
        LoadScene(indexSceneToLoad);
    }
    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        LoadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            //LoadingFill.fillAmount = Mathf.Clamp01(operation.progress / 0.9f);

            yield return null;
        }
    }
}
