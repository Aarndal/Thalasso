using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class EndCutscene : MonoBehaviour
{
    [SerializeField] private SO_GameInputReader _input;
    [SerializeField] private Transform cutscneeCamTransform;
    [SerializeField] private DoorAnimation escapePodDoor;
    [SerializeField] private float camTransitionDurations = 1f;
    private GameObject playerBody;
    private GameObject endCutsceneCanvas;
    private ButtonActions buttonActions;
    private CinemachineCamera cinemachineCamera;

    private void OnEnable()
    {
        if (endCutsceneCanvas == null)
            endCutsceneCanvas = GameObject.Find("EndCutsceneCanvas");

        if (buttonActions == null)
            buttonActions = endCutsceneCanvas.GetComponent<ButtonActions>();

        if (playerBody == null)
            playerBody = GameObject.FindGameObjectWithTag("PlayerBody");
    }
    private void OnDisable()
    {
        _input.SkipIsTriggered -= OnSkipCutScene;
    }
    private void Start()
    {
        cutscneeCamTransform.gameObject.SetActive(false);
        cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
    }
    private void TransitionCamForEndcut()
    {

        StartCoroutine(TransformTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, cutscneeCamTransform, camTransitionDurations, null, 
            () =>
            {
                playerBody.SetActive(false);
                _input.SwitchCurrentActionMapTo("Cutscene");
                cinemachineCamera.enabled = false;
                escapePodDoor.TryClose();
                _input.SkipIsTriggered += OnSkipCutScene;
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
        buttonActions.LoadScene(2); //crdits scene
    }
}
