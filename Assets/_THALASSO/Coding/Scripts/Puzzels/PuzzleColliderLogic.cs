using System.Reflection;
using Unity.Cinemachine;
using UnityEngine;

public class PuzzleColliderLogic : MonoBehaviour, IAmInteractive
{
    [Header("References")]
    [SerializeField] private SO_GameInputReader _input;

    [SerializeField] private string playerTag;
    [SerializeField] private int puzzleID;
    [SerializeField] private Transform targetCamera;
    [SerializeField] private float transitionduration;
    [SerializeField] private AnimationCurve animationSpeedCurve;

    [SerializeField] private bool puzzleAutoStartNeeded = false;
    [SerializeField] private MonoBehaviour autoStartPuzzleScript;

    private GameObject buttonUICanvas;
    private bool isfocused = false;
    private Vector3 originTransformPosition;
    private Quaternion originTransformRotation;
    private GameObject tempGameObject;
    private CinemachineCamera cinemachineVirtualCamera;
    private bool inAnimation;

    private bool isActivatable = true;
    public bool IsActivatable => isActivatable;

    private void Awake()
    {
        PuzzleUIReferencesSender.puzzleUIReferenceLogger += GetUIReference;
    }

    private void GetUIReference(GameObject reference, int ID)
    {
        if (ID == puzzleID)
        {
            buttonUICanvas = reference;
            buttonUICanvas.SetActive(false);
        }
    }

    private void Start()
    {
        cinemachineVirtualCamera = FindAnyObjectByType<CinemachineCamera>();
    }

    public void Interact(Transform transform)
    {
        if (inAnimation)
            return;
        if (!isfocused)
        {
            originTransformPosition = Camera.main.transform.position;
            originTransformRotation = Camera.main.transform.rotation;

            StartCoroutine(TransformTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, targetCamera, transitionduration, animationSpeedCurve, () =>
            {
                inAnimation = true;
                cinemachineVirtualCamera.enabled = false;
            }, () =>
            {
                inAnimation = false;
                buttonUICanvas.SetActive(true);
                isfocused = true;

                if (puzzleAutoStartNeeded && autoStartPuzzleScript != null)
                {
                    MethodInfo method = autoStartPuzzleScript.GetType().GetMethod("StartPuzzle");
                    method?.Invoke(autoStartPuzzleScript, null);
                }
            }));

            _input.SwitchCurrentActionMapTo("UI"); // Switch to UI ActionMap and disable any other Action Map
        }
        else
        {
            Transform tempOriginalTransform = PosRotToTransform(originTransformPosition, originTransformRotation);

            StartCoroutine(TransformTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, tempOriginalTransform, transitionduration, animationSpeedCurve, () =>
            {
                inAnimation = true;
                buttonUICanvas.SetActive(false);
            }, () =>
            {
                inAnimation = false;
                cinemachineVirtualCamera.enabled = true;
                isfocused = false;
            }));
            
            _input.SwitchCurrentActionMapTo("Player"); // Switch to Player ActionMap and disable any other Action Map
        }
    }

    public Transform PosRotToTransform(Vector3 position, Quaternion rotation)
    {
        if (tempGameObject == null)
            tempGameObject = new GameObject("temp");

        tempGameObject.transform.SetPositionAndRotation(position, rotation);
        return tempGameObject.transform;
    }

    private void OnDestroy()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
