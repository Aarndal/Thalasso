using UnityEngine;

public class DoorAnimationTrigger : MonoBehaviour
{

    [Header("General")]
    [SerializeField] private GameObject doorObjL;
    [SerializeField] private GameObject doorObjR;
    [SerializeField] private float openingDistance;
    private Vector3 originTransformPositionL;
    private Vector3 originTransformPositionR;
    private Coroutine runningCoroutineAnimationL;
    private Coroutine runningCoroutineAnimationR;
    [SerializeField] private bool isLocked = false;


    [Header("Opening")]
    [SerializeField] private float openingDuration;
    [SerializeField] private AnimationCurve openingSpeedCurve;

    [Header("Opening")]
    [SerializeField] private float closingDuration;
    [SerializeField] private AnimationCurve closingSpeedCurve;

    public bool IsLocked => isLocked;

    private void Start()
    {
        originTransformPositionL = doorObjL.transform.position;
        originTransformPositionR = doorObjR.transform.position;
    }

    public void OpenDoor()
    {
        if (isLocked)
            return;

        Vector3 endPosL;
        Vector3 endPosR;
        endPosL = originTransformPositionL + (-doorObjL.transform.right * openingDistance);
        endPosR = originTransformPositionR + (doorObjR.transform.right * openingDistance);

        if (runningCoroutineAnimationL != null || runningCoroutineAnimationR != null)
        {
            StopCoroutine(runningCoroutineAnimationL);
            StopCoroutine(runningCoroutineAnimationR);
        }
        runningCoroutineAnimationL = TransformTransitionSystem.Instance.TransitionPos(doorObjL, endPosL, openingDuration, openingSpeedCurve, null, () => runningCoroutineAnimationL = null);
        runningCoroutineAnimationR = TransformTransitionSystem.Instance.TransitionPos(doorObjR, endPosR, openingDuration, openingSpeedCurve, null, () => runningCoroutineAnimationR = null);
    }
    public void CloseDoor()
    {

        if (isLocked)
            return;

        if (runningCoroutineAnimationL != null || runningCoroutineAnimationR != null)
        {
            StopCoroutine(runningCoroutineAnimationL);
            StopCoroutine(runningCoroutineAnimationR);
        }
        runningCoroutineAnimationL = TransformTransitionSystem.Instance.TransitionPos(doorObjL, originTransformPositionL, closingDuration, closingSpeedCurve, null, () => runningCoroutineAnimationL = null);
        runningCoroutineAnimationR = TransformTransitionSystem.Instance.TransitionPos(doorObjR, originTransformPositionR, closingDuration, closingSpeedCurve, null, () => runningCoroutineAnimationR = null);
    }

    public void Unlock()
    {
        if (isLocked)
            isLocked = false;
    }

    public void Lock()
    {
        if (!isLocked)
            isLocked = true;
    }
}
