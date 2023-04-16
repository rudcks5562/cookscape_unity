using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour
{
    public static string nextScene;
    [SerializeField] Slider progressBar;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(0.2f);

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float gauge = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            gauge += Time.deltaTime;
            if (op.progress < 0.9f) {
                progressBar.value = Mathf.Lerp(progressBar.value, op.progress, gauge);
                if (progressBar.value >= op.progress)
                {
                    gauge = 0f;
                }
            } else {
                progressBar.value = Mathf.Lerp(progressBar.value, 1f, gauge);
                if (progressBar.value == 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
