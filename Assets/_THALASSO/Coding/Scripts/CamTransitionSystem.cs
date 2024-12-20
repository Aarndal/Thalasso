using System.Collections;
using UnityEngine;

public class CamTransitionSystem : MonoBehaviour
{
    public static CamTransitionSystem Instance;
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

    public Coroutine TransitionPosRot(GameObject _actor, Transform _target, float _duration, AnimationCurve _speedCurve = null, System.Action _onStartEvent = null, System.Action _onCompleteEvent = null)
    {
        return StartCoroutine(ETransitionPosRot(_actor, _target, _duration, _speedCurve, _onStartEvent, _onCompleteEvent));
    }
    public  Coroutine TransitionPos(GameObject _actor, Transform _target, float _duration, AnimationCurve _speedCurve = null, System.Action _onStartEvent = null, System.Action _onCompleteEvent = null)
    {
        return StartCoroutine(ETransitionPos(_actor, _target, _duration, _speedCurve, _onStartEvent, _onCompleteEvent));
    }
    public Coroutine TransitionRot(GameObject _actor, Transform _target, float _duration, AnimationCurve _speedCurve = null, System.Action _onStartEvent = null, System.Action _onCompleteEvent = null)
    {
        return StartCoroutine(ETransitionRot(_actor, _target, _duration, _speedCurve, _onStartEvent, _onCompleteEvent));
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    #region IEnumerators
    private IEnumerator ETransitionPosRot(GameObject _actor, Transform _target, float _duration, AnimationCurve _speedCurve, System.Action _onStart, System.Action _onComplete)
    {
        if (_onStart != null)
        {
            _onStart.Invoke();
        }

        Vector3 startPosition = _actor.transform.position;
        Quaternion startRotation = _actor.transform.rotation;

        Vector3 targetPosition = _target.position;
        Quaternion targetRotation = _target.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _duration);

            float curveT = _speedCurve != null ? _speedCurve.Evaluate(t) : t;

            _actor.transform.position = Vector3.Lerp(startPosition, targetPosition, curveT);
            _actor.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, curveT);

            yield return null;
        }

        _actor.transform.position = targetPosition;
        _actor.transform.rotation = targetRotation;

        if (_onComplete != null)
        {
            _onComplete.Invoke();
        }
    }

    private IEnumerator ETransitionPos(GameObject _actor, Transform _target, float _duration, AnimationCurve _speedCurve, System.Action _onStart, System.Action _onComplete)
    {
        if (_onStart != null)
        {
            _onStart.Invoke();
        }

        Vector3 startPosition = _actor.transform.position;

        Vector3 targetPosition = _target.position;

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

        if (_onComplete != null)
        {
            _onComplete.Invoke();
        }
    }
    private IEnumerator ETransitionRot(GameObject _actor, Transform _target, float _duration, AnimationCurve _speedCurve, System.Action _onStart, System.Action _onComplete)
    {
        if (_onStart != null)
        {
            _onStart.Invoke();
        }

        Quaternion startRotation = _actor.transform.rotation;

        Quaternion targetRotation = _target.rotation;

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

        if (_onComplete != null)
        {
            _onComplete.Invoke();
        }
    }
    #endregion
}
