using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

#if WWISE_2024_OR_LATER
[RequireComponent(typeof(AkGameObj))]
#endif
public class DoorAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Left or Bottom Door")]
    private GameObject leftOrBottomDoor = default;
    [SerializeField, Tooltip("Right or Upper Door")]
    private GameObject rightOrUpperDoor = default;

    [Space(10)]

    [Header("Settings")]

    // Door States
    [SerializeField] private bool isOpen = false;
    private bool inTransition = false;
    private bool isOpening = false;
    private bool isClosing = false;

    [SerializeField]
    private bool isLocked = false;
    [SerializeField]
    private bool isAutomatic = true;
    [SerializeField, Min(0.0f)]
    private float openingTime = 5.0f;
    [SerializeField]
    private DoorAnimationType doorAnimationType = DoorAnimationType.None;
    [SerializeField]
    private Axis localAxis = Axis.None;
    [SerializeField]
    private bool openInSameDirection = false;
    [SerializeField]
    private float openingDistance = 6.5f;
    [SerializeField, Range(-180.0f, 180.0f)]
    private float openingRotation = 120.0f;

    // Door Origin
    private Vector3 originPositionL;
    private Vector3 originPositionR;
    private Quaternion originRotationL;
    private Quaternion originRotationR;

    [Space(10)]

    [Header("Opening Animation")]
    [SerializeField]
    private float openingDuration;
    [SerializeField]
    private AnimationCurve openingSpeedCurve;

    [Header("Closing Animation")]
    [SerializeField]
    private float closingDuration;
    [SerializeField]
    private AnimationCurve closingSpeedCurve;

#if WWISE_2024_OR_LATER
    [Space(10)]

    [Header("Wwise Events")]
    [SerializeField]
    private AK.Wwise.Event _openDoorSound = default;
    [SerializeField]
    private AK.Wwise.Event _closeDoorSound = default;
#endif

    // Door Properties
    public bool InTransition => inTransition;
    public bool IsAutomatic => isAutomatic;
    public bool IsClosing => isClosing;
    public bool IsOpen => isOpen;
    public bool IsOpening => isOpening;
    public bool IsLocked
    {
        get => isLocked;
        private set
        {
            if (value != isLocked)
            {
                isLocked = value;
                if (isLocked)
                    isOpen = false;
            }
        }
    }

    // Public Door Events
    public event Action IsBeingOpened;
    public event Action HasBeenOpened;
    public event Action IsBeingClosed;
    public event Action HasBeenClosed;

    #region Unity Lifecycle Methods
    private void OnEnable()
    {
        IsBeingOpened += () =>
        {
            SetDoorStates(false, true, false);
#if WWISE_2024_OR_LATER
            _openDoorSound.Post(gameObject); 
#endif
        };
        IsBeingClosed += () =>
        {
            SetDoorStates(false, false, true);
#if WWISE_2024_OR_LATER
            _closeDoorSound.Post(gameObject);
#endif
        };
        HasBeenOpened += () => { SetDoorStates(true, false, false); };
        HasBeenClosed += () => SetDoorStates(false, false, true);
    }

    private void Start()
    {
        if (IsOpen)
            IsLocked = false;

        if (leftOrBottomDoor == null && rightOrUpperDoor == null)
        {
            Debug.LogErrorFormat("<color=red>DoorAnimation</color> component {0} (ID: {1}) <color=red>has no Door assigned!</color>", gameObject.name, gameObject.GetInstanceID());
            return;
        }

        if (leftOrBottomDoor != null)
        {
            originPositionL = leftOrBottomDoor.transform.position;
            originRotationL = leftOrBottomDoor.transform.rotation;
        }

        if (rightOrUpperDoor != null)
        {
            originPositionR = rightOrUpperDoor.transform.position;
            originRotationR = rightOrUpperDoor.transform.rotation;
        }
    }

    private void OnDisable()
    {
        IsBeingOpened -= () =>
        {
            SetDoorStates(false, true, false);
#if WWISE_2024_OR_LATER
            _openDoorSound.Post(gameObject); 
#endif
        };
        IsBeingClosed -= () => SetDoorStates(false, false, true);
        HasBeenOpened -= () => { SetDoorStates(true, false, false); };
        HasBeenClosed -= () => SetDoorStates(false, false, true);
    }
    #endregion

    #region Public Methods
    public bool TryOpen()
    {
        if (IsLocked || IsOpen || IsOpening)
            return false;

        StopAllCoroutines();
        StartCoroutine(Open());
        return true;
    }

    public bool TryClose()
    {
        if (!IsAutomatic && (!IsClosing || IsOpen))
        {
            StopAllCoroutines();
            StartCoroutine(Close());
            return true;
        }

        return false;
    }
    #endregion

    #region Enumerator Methods
    private IEnumerator Open()
    {
        switch (doorAnimationType)
        {
            case DoorAnimationType.Translation:
                OpenByTranslation();
                break;
            case DoorAnimationType.Rotation:
                OpenByRotation();
                break;
            case DoorAnimationType.TranslationAndRotation:
                OpenByTranslationAndRotation();
                break;
            default:
                break;
        }

        if (IsAutomatic)
            StartCoroutine(Close());

        yield return null;
    }

    private IEnumerator Close()
    {
        if (IsAutomatic)
        {
            yield return new WaitUntil(() => !InTransition);

            yield return new WaitForSeconds(openingTime);
        }

        switch (doorAnimationType)
        {
            case DoorAnimationType.Translation:
                CloseByTranslation();
                break;
            case DoorAnimationType.Rotation:
                CloseByRotation();
                break;
            case DoorAnimationType.TranslationAndRotation:
                CloseByTranslationAndRotation();
                break;
            default:
                break;
        }

        yield return null;
    }
    #endregion

    #region OpenDoor Methods
    private void OpenByTranslation()
    {
        if (openingDistance <= 0.0001f)
            Debug.LogErrorFormat("<color=red>Opening Distance</color> of {0} (ID: {1}) <color=red>is too small!</color>", gameObject.name, gameObject.GetInstanceID());

        if (leftOrBottomDoor != null)
        {
            float adjustOpeningDirection = openInSameDirection ? 1.0f : -1.0f;

            var endPosL = localAxis switch
            {
                Axis.X => originPositionL + (adjustOpeningDirection * openingDistance * leftOrBottomDoor.transform.right),
                Axis.Y => originPositionL + (adjustOpeningDirection * openingDistance * leftOrBottomDoor.transform.up),
                Axis.Z => originPositionL + (adjustOpeningDirection * openingDistance * leftOrBottomDoor.transform.forward),
                Axis.None => originPositionL + (adjustOpeningDirection * openingDistance * Vector3.one),
                _ => originPositionL + (adjustOpeningDirection * openingDistance * leftOrBottomDoor.transform.right),
            };

            StartCoroutine(TransformTransitionSystem.Instance.TransitionPos(leftOrBottomDoor, endPosL, openingDuration, openingSpeedCurve, IsBeingOpened, HasBeenOpened));
        }

        if (rightOrUpperDoor != null)
        {
            var endPosR = localAxis switch
            {
                Axis.X => originPositionR + (leftOrBottomDoor.transform.right * openingDistance),
                Axis.Y => originPositionR + (leftOrBottomDoor.transform.up * openingDistance),
                Axis.Z => originPositionR + (leftOrBottomDoor.transform.forward * openingDistance),
                Axis.None => originPositionR + (Vector3.one * openingDistance),
                _ => originPositionR + (leftOrBottomDoor.transform.right * openingDistance),
            };

            StartCoroutine(TransformTransitionSystem.Instance.TransitionPos(rightOrUpperDoor, endPosR, openingDuration, openingSpeedCurve, IsBeingOpened, HasBeenOpened));
        }
    }

    private void OpenByRotation()
    {
        if (openingRotation > -0.0001f && openingRotation < 0.0001f)
            Debug.LogErrorFormat("<color=red>Opening Rotation</color> of {0} (ID: {1}) <color=red>is too small!</color>", gameObject.name, gameObject.GetInstanceID());

        if (leftOrBottomDoor != null)
        {
            float adjustOpeningDirection = openInSameDirection ? 1.0f : -1.0f;

            var endPosL = localAxis switch
            {
                Axis.X => Quaternion.Euler(originRotationL.eulerAngles + adjustOpeningDirection * openingRotation * leftOrBottomDoor.transform.right),
                Axis.Y => Quaternion.Euler(originRotationL.eulerAngles + adjustOpeningDirection * openingRotation * leftOrBottomDoor.transform.up),
                Axis.Z => Quaternion.Euler(originRotationL.eulerAngles + adjustOpeningDirection * openingRotation * leftOrBottomDoor.transform.forward),
                Axis.None => Quaternion.Euler(originRotationL.eulerAngles + adjustOpeningDirection * openingRotation * Vector3.one),
                _ => Quaternion.Euler(originRotationL.eulerAngles + adjustOpeningDirection * openingRotation * leftOrBottomDoor.transform.up),
            };

            StartCoroutine(TransformTransitionSystem.Instance.TransitionRot(leftOrBottomDoor, endPosL, openingDuration, openingSpeedCurve, IsBeingOpened, HasBeenOpened));
        }

        if (rightOrUpperDoor != null)
        {
            var endPosR = localAxis switch
            {
                Axis.X => Quaternion.Euler(originRotationR.eulerAngles + rightOrUpperDoor.transform.right * openingRotation),
                Axis.Y => Quaternion.Euler(originRotationR.eulerAngles + rightOrUpperDoor.transform.up * openingRotation),
                Axis.Z => Quaternion.Euler(originRotationR.eulerAngles + rightOrUpperDoor.transform.forward * openingRotation),
                Axis.None => Quaternion.Euler(originRotationR.eulerAngles + Vector3.one * openingRotation),
                _ => Quaternion.Euler(originRotationR.eulerAngles + rightOrUpperDoor.transform.up * openingRotation),
            };

            StartCoroutine(TransformTransitionSystem.Instance.TransitionRot(rightOrUpperDoor, endPosR, openingDuration, openingSpeedCurve, IsBeingOpened, HasBeenOpened));
        }
    }

    private void OpenByTranslationAndRotation()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region CloseDoor Methods
    private void CloseByTranslation()
    {
        if (leftOrBottomDoor != null)
            StartCoroutine(TransformTransitionSystem.Instance.TransitionPos(leftOrBottomDoor, originPositionL, closingDuration, closingSpeedCurve, IsBeingClosed, HasBeenClosed));

        if (rightOrUpperDoor != null)
            StartCoroutine(TransformTransitionSystem.Instance.TransitionPos(rightOrUpperDoor, originPositionR, closingDuration, closingSpeedCurve, IsBeingClosed, HasBeenClosed));
    }

    private void CloseByRotation()
    {

        if (leftOrBottomDoor != null)
            StartCoroutine(TransformTransitionSystem.Instance.TransitionRot(leftOrBottomDoor, originRotationL, closingDuration, closingSpeedCurve, IsBeingClosed, HasBeenClosed));

        if (rightOrUpperDoor != null)
            StartCoroutine(TransformTransitionSystem.Instance.TransitionRot(rightOrUpperDoor, originRotationR, closingDuration, closingSpeedCurve, IsBeingClosed, HasBeenClosed));
    }

    private void CloseByTranslationAndRotation()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Lock/Unlock Methods
    public void Lock()
    {
        if (!IsLocked)
            IsLocked = true;
    }

    public void Unlock()
    {
        if (IsLocked)
            IsLocked = false;
    }
    #endregion

    private void SetDoorStates(bool isOpen, bool isOpening, bool isClosing)
    {
        this.isOpen = isOpen;
        this.isOpening = isOpening;
        this.isClosing = isClosing;

        if (isOpening || isClosing)
            inTransition = true;
        else
            inTransition = false;
    }
}
