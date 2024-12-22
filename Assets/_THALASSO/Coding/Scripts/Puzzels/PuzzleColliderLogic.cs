using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleColliderLogic : MonoBehaviour
{
    [SerializeField] private string playerTag;
    [SerializeField] private Transform targetCamera;
    [SerializeField] private float transitionduration;
    [SerializeField] private AnimationCurve animationSpeedCurve;
    [SerializeField] private GameObject buttonUICanvas;

    private bool inRange;
    private bool isfocused = false;
    private Vector3 originTransformPosition;
    private Quaternion originTransformRotation;
    private GameObject transform;
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Start()
    {
        buttonUICanvas.SetActive(false);
        cinemachineVirtualCamera = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>();

        InputManager.OnInteractGlobal += CheckCollision;
        
    }
    private void OnDisable()
    {
        InputManager.OnInteractGlobal -= CheckCollision;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            inRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            inRange = false;
        }
    }

    private void CheckCollision()
    {
        if (!inRange)
            return;


            if (!isfocused)
            {
                originTransformPosition = Camera.main.transform.position;
                originTransformRotation = Camera.main.transform.rotation;

                CamTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, targetCamera, transitionduration, animationSpeedCurve, () =>
                {
                    cinemachineVirtualCamera.enabled = false;
                    InputManager.Instance.BlockPlayerMoveAndRot();
                }, () =>
                {
                    Cursor.lockState = CursorLockMode.None;
                    buttonUICanvas.SetActive(true);
                    isfocused = true;
                });
            }
            else
            {
                Transform tempOriginalTransform = PosRotToTransform(originTransformPosition, originTransformRotation);

                CamTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, tempOriginalTransform, transitionduration, animationSpeedCurve, () =>
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    buttonUICanvas.SetActive(false);
                }, () =>
                {
                    cinemachineVirtualCamera.enabled = true;
                    isfocused = false;
                    InputManager.Instance.UnblockPlayerMoveAndRot();
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
