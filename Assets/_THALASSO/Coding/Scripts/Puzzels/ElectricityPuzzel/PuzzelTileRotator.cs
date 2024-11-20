using UnityEngine;
using System.Collections;

public class PuzzelTileRotator : MonoBehaviour
{
    public float rotationDuration = 1f; // Zeit in Sekunden für die Rotation
    private bool isRotating = false;

    public void OnRotateClick()
    {
        if (!isRotating) // Verhindert mehrfaches Starten
        {
            StartCoroutine(RotateOverTime(60, rotationDuration));
        }
    }

    private IEnumerator RotateOverTime(float angle, float duration)
    {
        isRotating = true;
        float elapsed = 0f;
        float rotationStep = angle / duration; // Rotation pro Sekunde

        while (elapsed < duration)
        {
            float deltaRotation = rotationStep * Time.deltaTime; // Rotation dieses Frames
            transform.Rotate(new Vector3(deltaRotation, 0, 0), Space.World);
            elapsed += Time.deltaTime;
            yield return null; // Wartet auf den nächsten Frame
        }

        // Sicherstellen, dass die Rotation exakt endet
        transform.Rotate(new Vector3(angle - (rotationStep * elapsed), 0, 0), Space.World);
        isRotating = false;
    }
}
