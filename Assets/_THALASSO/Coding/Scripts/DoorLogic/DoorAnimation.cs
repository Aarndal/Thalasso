using System;
using UnityEngine;
using UnityEngine.Animations;

public enum DoorAnimationType
{
    Translation,
    Rotation,
    TranslationAndRotation
}

public class DoorAnimation : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private DoorAnimationType doorAnimationType;
    [SerializeField] private Axis localAxis;
    [SerializeField] private GameObject doorObjL;
    [SerializeField] private GameObject doorObjR;
    [SerializeField] private bool areOpeningInSameDirection = false;
    [SerializeField] private float openingDistance;
    [SerializeField] private float openingRotation;
    private Vector3 originPositionL;
    private Vector3 originPositionR;
    private Quaternion originRotationL;
    private Quaternion originRotationR;
    private Coroutine runningCoroutineAnimationL;
    private Coroutine runningCoroutineAnimationR;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool isOpen = false;

    [Header("Opening")]
    [SerializeField] private float openingDuration;
    [SerializeField] private AnimationCurve openingSpeedCurve;

    [Header("Closing")]
    [SerializeField] private float closingDuration;
    [SerializeField] private AnimationCurve closingSpeedCurve;

    public bool IsLocked => isLocked;

    public event Action IsOpening;
    public event Action HasBeenOpened;
    public event Action IsClosing;
    public event Action HasBeenClosed;

    private void Awake()
    {
        if (isOpen)
            HasBeenOpened?.Invoke();
        else
            HasBeenClosed?.Invoke();
    }

    private void Start()
    {
        if (doorObjL != null)
        {
            originPositionL = doorObjL.transform.position;
            originRotationL = doorObjL.transform.rotation;
        }

        if (doorObjR != null)
        {
            originPositionR = doorObjR.transform.position;
            originRotationR = doorObjR.transform.rotation;
        }
    }

    #region OpenDoor
    public void OpenDoor()
    {
        if (isLocked)
            return;

        StopRunningCoroutines();

        switch (doorAnimationType)
        {
            case DoorAnimationType.Translation:
                OpenDoorTranslation();
                break;
            case DoorAnimationType.Rotation:
                OpenDoorRotation();
                break;
            case DoorAnimationType.TranslationAndRotation:
                OpenDoorTranslationAndRotation();
                break;
            default:
                break;
        }
    }

    private void OpenDoorTranslation()
    {
        if (doorObjL != null)
        {
            float adjustOpeningDirection = areOpeningInSameDirection ? 1.0f : -1.0f;

            var endPosL = localAxis switch
            {
                Axis.X => originPositionL + (doorObjL.transform.right * openingDistance * adjustOpeningDirection),
                Axis.Y => originPositionL + (doorObjL.transform.up * openingDistance * adjustOpeningDirection),
                Axis.Z => originPositionL + (doorObjL.transform.forward * openingDistance * adjustOpeningDirection),
                Axis.None => originPositionL + (Vector3.one * openingDistance * adjustOpeningDirection),
                _ => originPositionL + (-doorObjL.transform.right * openingDistance * adjustOpeningDirection),
            };

            runningCoroutineAnimationL = TransformTransitionSystem.Instance.TransitionPos(doorObjL, endPosL, openingDuration, openingSpeedCurve, IsOpening, HasBeenOpened);
        }

        if (doorObjR != null)
        {
            var endPosR = localAxis switch
            {
                Axis.X => originPositionR + (doorObjL.transform.right * openingDistance),
                Axis.Y => originPositionR + (doorObjL.transform.up * openingDistance),
                Axis.Z => originPositionR + (doorObjL.transform.forward * openingDistance),
                Axis.None => originPositionR + (Vector3.one * openingDistance),
                _ => originPositionR + (doorObjL.transform.right * openingDistance),
            };

            runningCoroutineAnimationR = TransformTransitionSystem.Instance.TransitionPos(doorObjR, endPosR, openingDuration, openingSpeedCurve, IsOpening, HasBeenOpened);
        }
    }

    private void OpenDoorRotation()
    {
        if (doorObjL != null)
        {
            float adjustOpeningDirection = areOpeningInSameDirection ? 1.0f : -1.0f;

            var endPosL = localAxis switch
            {
                Axis.X => Quaternion.Euler(originRotationL.eulerAngles + doorObjL.transform.right * openingRotation * adjustOpeningDirection),
                Axis.Y => Quaternion.Euler(originRotationL.eulerAngles + doorObjL.transform.up * openingRotation * adjustOpeningDirection),
                Axis.Z => Quaternion.Euler(originRotationL.eulerAngles + doorObjL.transform.forward * openingRotation * adjustOpeningDirection),
                Axis.None => Quaternion.Euler(originRotationL.eulerAngles + Vector3.one * openingRotation * adjustOpeningDirection),
                _ => Quaternion.Euler(originRotationL.eulerAngles + doorObjL.transform.up * openingRotation * adjustOpeningDirection),
            };
            runningCoroutineAnimationL = TransformTransitionSystem.Instance.TransitionRot(doorObjL, endPosL, openingDuration, openingSpeedCurve, IsOpening, HasBeenOpened);
        }

        if (doorObjR != null)
        {
            var endPosR = localAxis switch
            {
                Axis.X => Quaternion.Euler(originRotationR.eulerAngles + doorObjR.transform.right * openingRotation),
                Axis.Y => Quaternion.Euler(originRotationR.eulerAngles + doorObjR.transform.up * openingRotation),
                Axis.Z => Quaternion.Euler(originRotationR.eulerAngles + doorObjR.transform.forward * openingRotation),
                Axis.None => Quaternion.Euler(originRotationR.eulerAngles + Vector3.one * openingRotation),
                _ => Quaternion.Euler(originRotationR.eulerAngles + doorObjR.transform.up * openingRotation),
            };
            runningCoroutineAnimationR = TransformTransitionSystem.Instance.TransitionRot(doorObjR, endPosR, openingDuration, openingSpeedCurve, IsOpening, HasBeenOpened);
        }
    }

    private void OpenDoorTranslationAndRotation()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region CloseDoor
    public void CloseDoor()
    {
        if (isLocked)
            return;

        StopRunningCoroutines();

        switch (doorAnimationType)
        {
            case DoorAnimationType.Translation:
                CloseDoorTranslation();
                break;
            case DoorAnimationType.Rotation:
                CloseDoorRotation();
                break;
            case DoorAnimationType.TranslationAndRotation:
                CloseDoorTranslationAndRotation();
                break;
            default:
                break;
        }
    }

    private void CloseDoorTranslation()
    {
        if (doorObjL != null)
            runningCoroutineAnimationL = TransformTransitionSystem.Instance.TransitionPos(doorObjL, originPositionL, closingDuration, closingSpeedCurve, IsClosing, HasBeenClosed);

        if (doorObjR != null)
            runningCoroutineAnimationR = TransformTransitionSystem.Instance.TransitionPos(doorObjR, originPositionR, closingDuration, closingSpeedCurve, IsClosing, HasBeenClosed);
    }

    private void CloseDoorRotation()
    {

        if (doorObjL != null)
            runningCoroutineAnimationL = TransformTransitionSystem.Instance.TransitionRot(doorObjL, originRotationL, closingDuration, closingSpeedCurve, IsClosing, HasBeenClosed);

        if (doorObjR != null)
            runningCoroutineAnimationR = TransformTransitionSystem.Instance.TransitionRot(doorObjR, originRotationR, closingDuration, closingSpeedCurve, IsClosing, HasBeenClosed);
    }

    private void CloseDoorTranslationAndRotation()
    {
        throw new NotImplementedException();
    }
    #endregion

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

    private void StopRunningCoroutines()
    {
        if (runningCoroutineAnimationL != null || runningCoroutineAnimationR != null)
        {
            StopCoroutine(runningCoroutineAnimationL);
            StopCoroutine(runningCoroutineAnimationR);
        }
    }
}
