using System;
using System.Collections;
using UnityEngine;

public class PuzzleTileRotator : MonoBehaviour
{
    public float rotationDuration = 1f;
    private bool isRotating = false;

    public int curRotation = 0;

    public static event Action<GameObject> tileWasUpdated;

    public void OnRotateClick()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateOverTime(-60, rotationDuration));
        }
    }

    private IEnumerator RotateOverTime(int _rotationAngle, float _duration)
    {
        isRotating = true;
        float elapsed = 0f;
        float rotationStep = _rotationAngle / _duration;

        while (elapsed < _duration)
        {
            float deltaRotation = rotationStep * Time.deltaTime;
            transform.Rotate(transform.up * deltaRotation, Space.World);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.Rotate(transform.up * (_rotationAngle - (rotationStep * elapsed)), Space.World);
        isRotating = false;

        curRotation += _rotationAngle;
        if (curRotation >= 360 || curRotation <= -360)
            curRotation = 0;

        tileWasUpdated.Invoke(this.gameObject);
    }
}
