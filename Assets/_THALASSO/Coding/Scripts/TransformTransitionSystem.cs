using System.Collections;
using UnityEngine;

public class TransformTransitionSystem : MonoBehaviour
{
    public static TransformTransitionSystem Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    //public Coroutine TransitionPosRot(GameObject _actor, Transform _target, float _duration, AnimationCurve _speedCurve = null, System.Action _onStartEvent = null, System.Action _onCompleteEvent = null)
    //{
    //    return StartCoroutine(ETransitionPosRot(_actor, _target, _duration, _speedCurve, _onStartEvent, _onCompleteEvent));
    //}
    //public  Coroutine TransitionPos(GameObject _actor, Vector3 _target, float _duration, AnimationCurve _speedCurve = null, System.Action _onStartEvent = null, System.Action _onCompleteEvent = null)
    //{
    //    return StartCoroutine(ETransitionPos(_actor, _target, _duration, _speedCurve, _onStartEvent, _onCompleteEvent));
    //}
    //public Coroutine TransitionRot(GameObject _actor, Quaternion _target, float _duration, AnimationCurve _speedCurve = null, System.Action _onStartEvent = null, System.Action _onCompleteEvent = null)
    //{
    //    return StartCoroutine(ETransitionRot(_actor, _target, _duration, _speedCurve, _onStartEvent, _onCompleteEvent));
    //}

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    #region IEnumerators
    public IEnumerator TransitionPosRot(GameObject _actor, Transform _target, float _duration, AnimationCurve _speedCurve = null, System.Action _onStart = null, System.Action _onComplete = null)
    {
        _onStart?.Invoke();

        _actor.transform.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);
        _target.GetPositionAndRotation(out Vector3 targetPosition, out Quaternion targetRotation);

        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _duration);

            float curveT = _speedCurve != null ? _speedCurve.Evaluate(t) : t;

            _actor.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, targetPosition, curveT), Quaternion.Lerp(startRotation, targetRotation, curveT));
            yield return null;
        }

        _actor.transform.SetPositionAndRotation(targetPosition, targetRotation);
        _onComplete?.Invoke();
    }

    public IEnumerator TransitionPos(GameObject _actor, Vector3 _target, float _duration, AnimationCurve _speedCurve = null, System.Action _onStart = null, System.Action _onComplete = null)
    {
        _onStart?.Invoke();

        Vector3 startPosition = _actor.transform.position;

        Vector3 targetPosition = _target;

        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _duration);

            float curveT = _speedCurve != null ? _speedCurve.Evaluate(t) : t;

            _actor.transform.position = Vector3.Lerp(startPosition, targetPosition, curveT);

            yield return null;
        }

        _actor.transform.position = targetPosition;

        _onComplete?.Invoke();
    }
    public IEnumerator TransitionRot(GameObject _actor, Quaternion _target, float _duration, AnimationCurve _speedCurve = null, System.Action _onStart = null, System.Action _onComplete = null)
    {
        _onStart?.Invoke();

        Quaternion startRotation = _actor.transform.rotation;

        Quaternion targetRotation = _target;

        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _duration);

            float curveT = _speedCurve != null ? _speedCurve.Evaluate(t) : t;

            _actor.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, curveT);

            yield return null;
        }

        _actor.transform.rotation = targetRotation;

        _onComplete?.Invoke();
    }
    #endregion
}
