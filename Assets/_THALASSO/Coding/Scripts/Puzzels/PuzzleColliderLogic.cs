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
    
    private IAmRiddle autoStartPuzzleScript;

    private GameObject buttonUICanvas;
    private bool isfocused = false;
    private Vector3 originTransformPosition;
    private Quaternion originTransformRotation;
    private GameObject transform;
    private CinemachineCamera cinemachineCamera;
    private bool inAnimation = false;

    private readonly bool isActivatable = true;

    public bool IsActivatable => isActivatable;

    private void Awake()
    {
        autoStartPuzzleScript ??= GetComponentInChildren<IAmRiddle>();

        PuzzleUIReferencesSender.PuzzleUIReferenceLogger += GetUIReference;
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
        cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
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
                cinemachineCamera.enabled = false;
            }, () =>
            {
                inAnimation = false;
                buttonUICanvas.SetActive(true);
                isfocused = true;

                if (puzzleAutoStartNeeded && autoStartPuzzleScript != null)
                {
                    autoStartPuzzleScript.StartPuzzle();
                    //method?.Invoke(autoStartPuzzleScript, null);
                }
            }));

            _input.SwitchCurrentActionMap("UI"); // Switch to UI ActionMap and disable any other Action Map
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
                cinemachineCamera.enabled = true;
                isfocused = false;
            }));

            _input.SwitchCurrentActionMap("Player"); // Switch to Player ActionMap and disable any other Action Map
        }
    }

    public Transform PosRotToTransform(Vector3 position, Quaternion rotation)
    {
        if (transform == null)
            transform = new GameObject("temp");

        transform.transform.SetPositionAndRotation(position, rotation);
        return transform.transform;
    }

    private void OnDestroy()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
