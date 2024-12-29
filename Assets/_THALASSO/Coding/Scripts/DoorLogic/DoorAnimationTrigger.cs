using UnityEngine;

public class DoorAnimationTrigger : MonoBehaviour
{
    private enum directions
    {
        Left,
        Right,
        Up,
        Down
    }

    [Header("General")]
    [SerializeField] private GameObject doorObj;
    [SerializeField] private float openingDistance;
    [SerializeField] private directions openingDirection;
    private Vector3 originTransformPosition;
    private Coroutine runningCoroutineAnimation;


    [Header("Opening")]
    [SerializeField] private float openingDuration;
    [SerializeField] private AnimationCurve openingSpeedCurve;

    [Header("Opening")]
    [SerializeField] private float closingDuration;
    [SerializeField] private AnimationCurve closingSpeedCurve;

    private void Start()
    {
        originTransformPosition = doorObj.transform.position;
    }

    public void OpenDoor()
    {
        Vector3 endPos;
        switch (openingDirection)
        {
            case directions.Left:
                {
                    endPos = originTransformPosition + (-doorObj.transform.right * openingDistance);
                    break;
                }

            case directions.Right:
                {
                    endPos = originTransformPosition + (doorObj.transform.right * openingDistance);
                    break;
                }

            case directions.Up:
                {

                    endPos = originTransformPosition + (doorObj.transform.up * openingDistance);
                    break;
                }

            case directions.Down:
                {
                    endPos = originTransformPosition + (-doorObj.transform.up * openingDistance);
                    break;
                }

            default:
                {
                    endPos = Vector3.zero;
                    break;
                }
        }
        if (runningCoroutineAnimation != null)
        {
            StopCoroutine(runningCoroutineAnimation);
        }
        runningCoroutineAnimation = TransformTransitionSystem.Instance.TransitionPos(doorObj, endPos, openingDuration, openingSpeedCurve, null,() => runningCoroutineAnimation = null);
    }
    public void CloseDoor()
    {
        if (runningCoroutineAnimation != null)
        {
            StopCoroutine(runningCoroutineAnimation);
        }
        runningCoroutineAnimation = TransformTransitionSystem.Instance.TransitionPos(doorObj, originTransformPosition, closingDuration, closingSpeedCurve, null, () => runningCoroutineAnimation = null);
    }
}
