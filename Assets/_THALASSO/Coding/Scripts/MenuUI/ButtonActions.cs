using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonActions : MonoBehaviour
{

    [SerializeField] private bool useFade = true;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    [HideInInspector] public GameObject pauseMenuToggle;

    private void Start()
    {
        if (fadeImage != null && useFade)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeIn());
        }
        try
        {
            pauseMenuToggle = transform.Find("Toggle").gameObject;
            pauseMenuToggle.gameObject.SetActive(false);
        }
        catch 
        { 
            pauseMenuToggle = null;
        }
    }

    #region NormalUIButtonActions
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void PauseGame()
    {
        pauseMenuToggle.gameObject.SetActive(true);
        //ToDo: block normal input
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void ResumeGame()
    {
        pauseMenuToggle.gameObject.SetActive(false);
        //ToDo: unblock normal input
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    #region SceneLoadingButtonActions
    public void LoadScene(int sceneId)
    {
        if (sceneId == 3) //mainGame
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        if (useFade)
        {
            StartCoroutine(LoadSceneWithFade(sceneId));
        }
        else
        {
            LoadSceneWithoutFade(sceneId);
        }
    }
    public void UnloadScene(int sceneId)
    {
        SceneManager.UnloadSceneAsync(sceneId);
    }

    private IEnumerator LoadSceneWithoutFade(int sceneId)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneId);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneId));

    }
    private IEnumerator LoadSceneWithFade(int sceneId)
    {
        yield return FadeOut();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneId);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneId));
    }

    public void LoadScenes(int[] sceneIDs)
    {
        foreach (int sceneId in sceneIDs)
        {
            SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
        }
    }
    #endregion

    #region SmoothAnimation
    private IEnumerator FadeOut()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1);
        }
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage != null)
        {
            float t = fadeDuration;
            while (t > 0)
            {
                t -= Time.deltaTime;
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }
    }
    #endregion
}
