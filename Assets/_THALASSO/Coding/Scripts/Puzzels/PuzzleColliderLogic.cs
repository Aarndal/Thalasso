using System.Collections;
using UnityEngine;

public class PuzzleColliderLogic : MonoBehaviour
{
    [SerializeField] private string playerTag;
    [SerializeField] private Transform targetCamera;
    [SerializeField] private float transitionduration;
    [SerializeField] private AnimationCurve animationSpeedCurve;

    private Coroutine curCoroutine;
    private Transform originTransform;


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
                    originTransform.position = Camera.main.transform.position;
                    originTransform.rotation = Camera.main.transform.rotation;

                    CamTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, targetCamera, transitionduration, animationSpeedCurve, null, () => isfocused = true);
                }
                else
                {
                    CamTransitionSystem.Instance.TransitionPosRot(Camera.main.gameObject, originTransform, transitionduration, animationSpeedCurve, null, () => isfocused = false);
                }
            }
            yield return null;
        }
    }
}
