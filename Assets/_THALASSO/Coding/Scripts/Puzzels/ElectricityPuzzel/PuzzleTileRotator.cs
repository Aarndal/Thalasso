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
            StartCoroutine(RotateOverTime(60, rotationDuration));
        }
    }

    public void ResetTile()
    {
        StopAllCoroutines();

        curRotation = 0;
    }

    private IEnumerator RotateOverTime(int _angle, float _duration)
    {
        isRotating = true;
        float elapsed = 0f;
        float rotationStep = _angle / _duration;

        while (elapsed < _duration)
        {
            float deltaRotation = rotationStep * Time.deltaTime;
            transform.Rotate(new Vector3(deltaRotation, 0, 0), Space.Self);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.Rotate(new Vector3(_angle - (rotationStep * elapsed), 0, 0), Space.Self);
        isRotating = false;

        curRotation += _angle;
        if (curRotation > 360)
            curRotation -= 360;

        tileWasUpdated.Invoke(this.gameObject);
    }
}
