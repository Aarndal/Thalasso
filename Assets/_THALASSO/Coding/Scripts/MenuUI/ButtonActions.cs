using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonActions : MonoBehaviour
{
    [HideInInspector] public GameObject pauseMenuToggle;

    [Header("References")]
    [SerializeField] private SO_GameInputReader input = default;

    [Header("Fade Settings")]
    [SerializeField] private bool useFade = true;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;


#if WWISE_2024_OR_LATER
    [Header("Audio Settings")]
    [SerializeField] private AK.Wwise.Event enterMainMenuSound;
    [SerializeField] private AK.Wwise.Event exitMainMenuSound;
    [SerializeField] private AK.Wwise.Event enterPauseMenuSound;
    [SerializeField] private AK.Wwise.Event exitPauseMenuSound;
#endif

    private void Awake()
    {
        int curSceneId = SceneManager.GetActiveScene().buildIndex;

        if (curSceneId == 0) //MainMenu
        {
            input.SwitchCurrentActionMap("UI");
        }
        else if (curSceneId == 2) //Credits
        {
            input.SwitchCurrentActionMap("Cutscene");
        }
    }

    private void OnEnable()
    {
        input.PauseIsPerformed += OnPauseIsPerformed;
    }

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
            pauseMenuToggle.SetActive(false);
        }
        catch
        {
            pauseMenuToggle = null;
        }
    }
    private void OnDisable()
    {
        input.PauseIsPerformed -= OnPauseIsPerformed;
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

    private void OnPauseIsPerformed() => TogglePause();

    public void TogglePause()
    {
        if (pauseMenuToggle == null)
            return;

        if (input.IsPauseActive)
        {
#if WWISE_2024_OR_LATER
            enterPauseMenuSound.Post(gameObject);
#endif
            pauseMenuToggle.SetActive(true);
            input.SwitchCurrentActionMap("UI"); // Switch to UI ActionMap and disable any other Action Map.
        }
        else
        {
#if WWISE_2024_OR_LATER
            exitPauseMenuSound.Post(gameObject);
#endif
            pauseMenuToggle.SetActive(false);
            input.SwitchCurrentActionMap(input.PreviousActionMap.name); // Switch to previous ActionMap before Pause and disable any other Action Map.
        }
    }

    public void ResumeGame()
    {
        input.IsPauseActive = false;

        TogglePause();
    }
    #endregion

    #region SceneLoadingButtonActions
    public void LoadScene(int sceneId)
    {
        if (SceneManager.GetSceneByBuildIndex(sceneId) == SceneManager.GetActiveScene())
            return;

        if (sceneId == 2) //Credits
        {
            input.SwitchCurrentActionMap("Cutscene"); // Switch to UI ActionMap and disable any other Action Map
        }
        else
        {
#if WWISE_2024_OR_LATER
            if (sceneId == 0)
                enterMainMenuSound.Post(gameObject);

            if (sceneId == 3)
                exitMainMenuSound.Post(gameObject);
#endif

            input.IsPauseActive = false;
            input.SwitchCurrentActionMap("UI"); // Switch to UI ActionMap and disable any other Action Map
        }

        if (useFade)
        {
            StartCoroutine(LoadSceneWithFade(sceneId));
        }
        else
        {
            StartCoroutine(LoadSceneWithoutFade(sceneId));
        }
    }
    //public void UnloadScene(int sceneId)
    //{
    //    SceneManager.UnloadSceneAsync(sceneId);
    //}

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

        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneId));

    }
    private IEnumerator LoadSceneWithFade(int sceneId)
    {
        yield return new WaitUntil(() => FadeOut());
        //SceneManager.LoadSceneAsync(sceneId);

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

        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneId));
    }

    //public void LoadScenes(int[] sceneIDs)
    //{
    //    foreach (int sceneId in sceneIDs)
    //    {
    //        SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
    //    }
    //}
    #endregion

    #region SmoothAnimation
    private bool FadeOut()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
            }
            fadeImage.color = new Color(0, 0, 0, 1);
            return true;
        }
        return true;
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
