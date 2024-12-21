using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleColliderLogic : MonoBehaviour
{
    [SerializeField] private string playerTag;
    [SerializeField] private Transform targetCamera;
    [SerializeField] private float transitionduration;
    [SerializeField] private AnimationCurve animationSpeedCurve;


    private PlayerInput playerInput;

    private Coroutine curCoroutine;
    private Vector3 originTransformPosition;
    private Quaternion originTransformRotation;
    private GameObject transform;
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Start()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        cinemachineVirtualCamera = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            curCoroutine = StartCoroutine(AcceptInput());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            StopCoroutine(curCoroutine);
        }
    }

    private IEnumerator AcceptInput()
    {
        bool isfocused = false;

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isfocused)
                {
                    originTransformPosition = Camera.main.transform.position;
                    originTransformRotation = Camera.main.transform.rotation;

                    CamTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, targetCamera, transitionduration, animationSpeedCurve, () =>
                    {
                        cinemachineVirtualCamera.enabled = false;
                        playerInput.DeactivateInput();
                    }, () =>
                    {
                        isfocused = true;
                    });
                }
                else
                {
                    CamTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, PosRotToTransform(originTransformPosition, originTransformRotation), transitionduration, animationSpeedCurve, null, () =>
                    {
                        cinemachineVirtualCamera.enabled = true;
                        isfocused = false;
                        playerInput.ActivateInput();
                    });
                }
            }
            yield return null;
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
