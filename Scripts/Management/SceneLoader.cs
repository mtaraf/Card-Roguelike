using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        loadingScreen = Resources.Load<GameObject>("UI/SceneUI/SceneLoadingScreen");
    }

    public void loadScene(string sceneName, Action onComplete = null)
    {
        StartCoroutine(loadSceneAsync(sceneName, onComplete));
    }

    public void loadScene(int sceneIndex, Action onComplete = null)
    {
        StartCoroutine(loadSceneAsync(SceneManager.GetSceneByBuildIndex(sceneIndex).name, onComplete));
    }

    IEnumerator loadSceneAsync(string sceneName, Action onComplete)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        // Optional delay or animation here
        yield return new WaitForSeconds(1f);


        yield return StartCoroutine(FadeOut(1.2f));
        operation.allowSceneActivation = true;
        yield return new WaitUntil(() => operation.isDone);

        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(FadeIn(3f));

        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        onComplete?.Invoke();
    }

    public IEnumerator FadeOut(float duration)
    {
        fadeImage.gameObject.SetActive(true);
        float time = 0;
        Color color = fadeImage.color;
        while (time < duration)
        {
            color.a = Mathf.Lerp(0, 1, time / duration);
            fadeImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        color.a = 1;
        fadeImage.color = color;
    }

    public IEnumerator FadeIn(float duration)
    {
        float time = 0;
        Color color = fadeImage.color;
        while (time < duration)
        {
            color.a = Mathf.Lerp(1, 0, time / duration);
            fadeImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        color.a = 0;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }
}