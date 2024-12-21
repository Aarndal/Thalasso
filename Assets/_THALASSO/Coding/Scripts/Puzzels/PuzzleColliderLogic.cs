using System.Collections;
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

    private void Start()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
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

                    CamTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, targetCamera, transitionduration, animationSpeedCurve, () => playerInput.ActivateInput(), () => isfocused = true);
                }
                else
                {
                    CamTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, PosRotToTransform(originTransformPosition,originTransformRotation), transitionduration, animationSpeedCurve, null, () =>
                    {
                        isfocused = false;
                        playerInput.DeactivateInput();
                    });
                }
            }
            yield return null;
        }
    }

    public Transform PosRotToTransform(Vector3 position, Quaternion rotation)
    {
        GameObject transform = new GameObject("temp");
        transform.transform.position = position;
        transform.transform.rotation = rotation;
        return transform.transform;
    }
}
