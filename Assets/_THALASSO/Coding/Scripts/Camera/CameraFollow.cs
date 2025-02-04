using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _cameraRoot;

    private void OnEnable()
    {
        transform.SetPositionAndRotation(_cameraRoot.position, _cameraRoot.rotation);
    }

    private void LateUpdate()
    {
        if (transform.position != _cameraRoot.position)
            transform.position = _cameraRoot.position;

        if (transform.rotation != _cameraRoot.rotation)
            transform.rotation = _cameraRoot.rotation;
    }
}
