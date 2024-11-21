using System.Collections;
using UnityEngine;

public class PuzzleTileRotator : MonoBehaviour
{
    public float rotationDuration = 1f;
    private bool isRotating = false;

    public void OnRotateClick()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateOverTime(60, rotationDuration));
        }
    }

    private IEnumerator RotateOverTime(float _angle, float _duration)
    {
        isRotating = true;
        float elapsed = 0f;
        float rotationStep = _angle / _duration;

        while (elapsed < _duration)
        {
            float deltaRotation = rotationStep * Time.deltaTime;
            transform.Rotate(new Vector3(deltaRotation, 0, 0), Space.World);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.Rotate(new Vector3(_angle - (rotationStep * elapsed), 0, 0), Space.World);
        isRotating = false;
    }
}
