using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _cameraRoot;

    private void OnEnable()
    {
        this.transform.position = _cameraRoot.position;
        this.transform.rotation = _cameraRoot.rotation;
    }

    private void LateUpdate()
    {
        if (this.transform.position != _cameraRoot.position)
            this.transform.position = _cameraRoot.position;

        if (this.transform.rotation != _cameraRoot.rotation)
            this.transform.rotation = _cameraRoot.rotation;
    }
}
