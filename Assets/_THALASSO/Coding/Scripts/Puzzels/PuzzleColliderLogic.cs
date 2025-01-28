using System.Reflection;
using Unity.Cinemachine;
using UnityEngine;

public class PuzzleColliderLogic : MonoBehaviour, IAmInteractive
{
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
    private GameObject transform;
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
        cinemachineVirtualCamera = GameObject.FindAnyObjectByType<CinemachineCamera>();
    }

    public void Interact(Transform transform)
    {
        if (inAnimation)
            return;
        if (!isfocused)
        {
            originTransformPosition = Camera.main.transform.position;
            originTransformRotation = Camera.main.transform.rotation;

            TransformTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, targetCamera, transitionduration, animationSpeedCurve, () =>
            {
                inAnimation = true;
                cinemachineVirtualCamera.enabled = false;
                //InputManager.Instance.BlockPlayerMoveAndRot();
            }, () =>
            {
                inAnimation = false;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                buttonUICanvas.SetActive(true);
                isfocused = true;

                if (puzzleAutoStartNeeded && autoStartPuzzleScript != null)
                {
                    MethodInfo method = autoStartPuzzleScript.GetType().GetMethod("StartPuzzle");
                    method?.Invoke(autoStartPuzzleScript, null);
                }
            });
        }
        else
        {
            Transform tempOriginalTransform = PosRotToTransform(originTransformPosition, originTransformRotation);

            TransformTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, tempOriginalTransform, transitionduration, animationSpeedCurve, () =>
            {
                inAnimation = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                buttonUICanvas.SetActive(false);
            }, () =>
            {
                inAnimation = false;
                cinemachineVirtualCamera.enabled = true;
                isfocused = false;
                //InputManager.Instance.UnblockPlayerMoveAndRot();
            });
        }
    }

    public Transform PosRotToTransform(Vector3 position, Quaternion rotation)
    {
        if (transform == null)
            transform = new GameObject("temp");

        transform.transform.position = position;
        transform.transform.rotation = rotation;
        return transform.transform;
    }

    private void OnDestroy()
    {
        if (transform != null)
        {
            Destroy(transform);
        }
    }

}
