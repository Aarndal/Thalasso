using UnityEngine;

public class PCShoulderRotation : MonoBehaviour
{
    [SerializeField]
    private SO_GameInputReader _input = default;
    [SerializeField]
    private Transform _cameraRoot = default;

    [SerializeField]
    private float _topClampAngle = 60.0f;
    [SerializeField]
    private float _bottomClampAngle = -60.0f;
    [SerializeField]
    private float _lerpFactor = 0.5f;

    private void OnEnable() =>
        _input.LookInputHasChanged += OnLookInputHasChanged;

    private void OnDisable() =>
        _input.LookInputHasChanged -= OnLookInputHasChanged;

    private void OnLookInputHasChanged(Vector2 lookInput, bool isCurrentDeviceMouse)
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(_cameraRoot.localRotation.eulerAngles.x, 0, 0), _lerpFactor);
    }
}
