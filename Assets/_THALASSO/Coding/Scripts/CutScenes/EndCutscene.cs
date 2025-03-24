using Eflatun.SceneReference;
using ProgressionTracking;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCutscene : MonoBehaviour
{
    [SerializeField] private SO_GameInputReader _input;
    [SerializeField] private SO_SolvableData progressInfoObject;
    [SerializeField] private Transform cutscneeCamTransform;
    [SerializeField] private DoorAnimation escapePodDoor;
    [SerializeField] private float camTransitionDurations = 1f;
    [SerializeField] private SceneReference _sceneToLoad = default;
    private GameObject playerBody;
    private GameObject cutsceneCanvas;
    private CinemachineCamera cinemachineCamera;
    private GameObject skipInfoText;

    private void OnEnable()
    {
        if (cutsceneCanvas == null)
            cutsceneCanvas = GameObject.Find("CutsceneCanvas");

        if (playerBody == null)
            playerBody = GameObject.FindGameObjectWithTag("PlayerBody");

        progressInfoObject.ValueChanged += CheckProgressionUpdate;
    }
    private void Start()
    {
        cutscneeCamTransform.gameObject.SetActive(false);
        cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        skipInfoText = cutsceneCanvas.GetComponentInChildren<TextMeshProUGUI>().gameObject;
    }
    

    private void OnDisable()
    {
        _input.SkipIsPerformed -= OnSkipCutScene;
        progressInfoObject.ValueChanged -= CheckProgressionUpdate;
    }
    private void CheckProgressionUpdate(uint _id, bool _isSolved)
    {
        if (_isSolved)
        {
            GetComponent<Animator>().enabled = true;
        }
    }

    private void TransitionCamForEndcut()
    {

        StartCoroutine(TransformTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, cutscneeCamTransform, camTransitionDurations, null, 
            () =>
            {
                PCAnimation pCAnimation = FindAnyObjectByType<PCAnimation>();
                pCAnimation._inCutscene = false;

                skipInfoText.SetActive(true);

                playerBody.SetActive(false);
                _input.SwitchCurrentActionMap("Cutscene");
                cinemachineCamera.enabled = false;
                escapePodDoor.TryClose();
                _input.SkipIsPerformed += OnSkipCutScene;
            }, () =>
        {
            cutscneeCamTransform.gameObject.SetActive(true);
            Camera.main.gameObject.SetActive(false);
        }));
    }

    private void OnSkipCutScene()
    {
        OnCutsceneEnd();
    }

    public void OnCutsceneEnd()
    {
        SceneManager.LoadScene(_sceneToLoad.BuildIndex);
    }
}
