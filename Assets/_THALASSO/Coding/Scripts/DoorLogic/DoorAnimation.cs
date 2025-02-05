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
    [SerializeField] private Axis rotationAxis;
    [SerializeField] private GameObject doorObjL;
    [SerializeField] private GameObject doorObjR;
    [SerializeField] private float openingDistance;
    [SerializeField] private float openingRotation;
    private Transform originTransformL;
    private Transform originTransformR;
    private Coroutine runningCoroutineAnimationL;
    private Coroutine runningCoroutineAnimationR;
    [SerializeField] private bool isLocked = false;

    [Header("Opening")]
    [SerializeField] private float openingDuration;
    [SerializeField] private AnimationCurve openingSpeedCurve;

    [Header("Closing")]
    [SerializeField] private float closingDuration;
    [SerializeField] private AnimationCurve closingSpeedCurve;

    public bool IsLocked => isLocked;

    private void Start()
    {
        if (doorObjL != null)
            originTransformL = doorObjL.transform;

        if (doorObjR != null)
            originTransformR = doorObjR.transform;
    }

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
            Vector3 endPosL;
            endPosL = originTransformL.position + (-doorObjL.transform.right * openingDistance);
            runningCoroutineAnimationL = TransformTransitionSystem.Instance.TransitionPos(doorObjL, endPosL, openingDuration, openingSpeedCurve, null, () => runningCoroutineAnimationL = null);
        }

        if (doorObjR != null)
        {
            Vector3 endPosR;
            endPosR = originTransformR.position + (doorObjR.transform.right * openingDistance);
            runningCoroutineAnimationR = TransformTransitionSystem.Instance.TransitionPos(doorObjR, endPosR, openingDuration, openingSpeedCurve, null, () => runningCoroutineAnimationR = null);
        }
    }

    private void OpenDoorRotation()
    {
        if (doorObjL != null)
        {
            var endPosL = rotationAxis switch
            {
                Axis.X => Quaternion.Euler(originTransformL.rotation.eulerAngles + Vector3.right * (-openingRotation)),
                Axis.Y => Quaternion.Euler(originTransformL.rotation.eulerAngles + Vector3.up * (-openingRotation)),
                Axis.Z => Quaternion.Euler(originTransformL.rotation.eulerAngles + Vector3.forward * (-openingRotation)),
                Axis.None => Quaternion.Euler(originTransformL.rotation.eulerAngles + Vector3.one * (-openingRotation)),
                _ => Quaternion.Euler(originTransformL.rotation.eulerAngles + Vector3.up * (-openingRotation)),
            };
            runningCoroutineAnimationL = TransformTransitionSystem.Instance.TransitionRot(doorObjL, endPosL, openingDuration, openingSpeedCurve, null, () => runningCoroutineAnimationL = null);
        }

        if (doorObjR != null)
        {
            var endPosR = rotationAxis switch
            {
                Axis.X => Quaternion.Euler(originTransformR.rotation.eulerAngles + Vector3.right * openingRotation),
                Axis.Y => Quaternion.Euler(originTransformR.rotation.eulerAngles + Vector3.up * openingRotation),
                Axis.Z => Quaternion.Euler(originTransformR.rotation.eulerAngles + Vector3.forward * openingRotation),
                Axis.None => Quaternion.Euler(originTransformR.rotation.eulerAngles + Vector3.one * openingRotation),
                _ => Quaternion.Euler(originTransformR.rotation.eulerAngles + Vector3.up * openingRotation),
            };
            runningCoroutineAnimationR = TransformTransitionSystem.Instance.TransitionRot(doorObjR, endPosR, openingDuration, openingSpeedCurve, null, () => runningCoroutineAnimationR = null);
        }
    }

    private void OpenDoorTranslationAndRotation()
    {
        throw new NotImplementedException();
    }

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
            runningCoroutineAnimationL = TransformTransitionSystem.Instance.TransitionPos(doorObjL, originTransformL.position, closingDuration, closingSpeedCurve, null, () => runningCoroutineAnimationL = null);

        if (doorObjR != null)
            runningCoroutineAnimationR = TransformTransitionSystem.Instance.TransitionPos(doorObjR, originTransformR.position, closingDuration, closingSpeedCurve, null, () => runningCoroutineAnimationR = null);
    }

    private void CloseDoorRotation()
    {

        if (doorObjL != null)
            runningCoroutineAnimationL = TransformTransitionSystem.Instance.TransitionRot(doorObjL, originTransformL.rotation, closingDuration, closingSpeedCurve, null, () => runningCoroutineAnimationL = null);

        if (doorObjR != null)
            runningCoroutineAnimationR = TransformTransitionSystem.Instance.TransitionRot(doorObjR, originTransformR.rotation, closingDuration, closingSpeedCurve, null, () => runningCoroutineAnimationR = null);
    }

    private void CloseDoorTranslationAndRotation()
    {
        throw new NotImplementedException();
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

    private void StopRunningCoroutines()
    {
        if (runningCoroutineAnimationL != null || runningCoroutineAnimationR != null)
        {
            StopCoroutine(runningCoroutineAnimationL);
            StopCoroutine(runningCoroutineAnimationR);
        }
    }
}
