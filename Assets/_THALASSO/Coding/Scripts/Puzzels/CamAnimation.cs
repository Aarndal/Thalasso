using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CamAnimation : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject playerCam;
    [SerializeField, Range(0.1f, 2f)] private float animDurationInSec = 0.5f;
    [SerializeField] private GameObject buttonUICanvas;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isMoving = false;

    public void FocusPuzzle()
    {
        if (!isMoving)
        {
            originalPosition = playerCam.transform.position;
            originalRotation = playerCam.transform.rotation;

            StartCoroutine(MoveToTransform(playerCam, targetTransform, animDurationInSec, null, () => buttonUICanvas.SetActive(true)));
        }
    }

    public void UnfocusPuzzle(CinemachineVirtualCamera cinemachineCamera)
    {
        if (!isMoving)
        {
            Transform originalTransform = new GameObject("OriginalTransform").transform;
            originalTransform.position = originalPosition;
            originalTransform.rotation = originalRotation;

            StartCoroutine(MoveToTransform(playerCam, originalTransform, animDurationInSec, () => { buttonUICanvas.SetActive(false); cinemachineCamera.enabled = true; }, null));

            Destroy(originalTransform.gameObject);
        }
    }

    private IEnumerator MoveToTransform(GameObject _movingObject, Transform _target, float _duration, System.Action onStart, System.Action onComplete)
    {
        if (onStart != null)
        {
            onStart.Invoke();
        }

        isMoving = true;
        Vector3 startPosition = _movingObject.transform.position;
        Quaternion startRotation = _movingObject.transform.rotation;

        Vector3 targetPosition = _target.position;
        Quaternion targetRotation = _target.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _duration);

            _movingObject.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            _movingObject.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        _movingObject.transform.position = targetPosition;
        _movingObject.transform.rotation = targetRotation;
        isMoving = false;

        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }
}
