using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonActions : MonoBehaviour
{
    [SerializeField]
    private SO_GameInputReader _input = default;

    [SerializeField] private bool useFade = true;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    [HideInInspector] public GameObject pauseMenuToggle;


    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            _input.SwitchCurrentActionMapTo("UI"); // Switch to UI ActionMap and disable any other Action Map
    }

    private void OnEnable()
    {
        _input.PauseIsPerformed += OnPauseIsPerformed;
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
            pauseMenuToggle.gameObject.SetActive(false);
        }
        catch
        {
            pauseMenuToggle = null;
        }
    }
    private void OnDisable()
    {
        _input.PauseIsPerformed -= OnPauseIsPerformed;
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

        if (_input.IsPauseActive)
        {
            pauseMenuToggle.SetActive(true);
            _input.SwitchCurrentActionMapTo("UI"); // Switch to UI ActionMap and disable any other Action Map.
        }
        else
        {
            pauseMenuToggle.SetActive(false);
            _input.SwitchCurrentActionMapTo(_input.PreviousActionMap.name); // Switch to previous ActionMap before Pause and disable any other Action Map.
        }
    }

    public void ResumeGame()
    {
        _input.IsPauseActive = false;

        TogglePause();
    }
    #endregion

    #region SceneLoadingButtonActions
    public void LoadScene(int sceneId)
    {
        if (sceneId == 3) //mainGame
        {
            _input.SwitchCurrentActionMapTo("Player"); // Switch to Player ActionMap and disable any other Action Map
        }
        else
        {
            _input.IsPauseActive = false;
            _input.SwitchCurrentActionMapTo("UI"); // Switch to UI ActionMap and disable any other Action Map
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
