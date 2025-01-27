using UnityEditor;
using UnityEngine;

public abstract class SO_ResettableData : ScriptableObject
{
    protected bool _isPlayMode;

    protected virtual void OnEnable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            _isPlayMode = true;

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    protected virtual void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
    {
        if (stateChange == PlayModeStateChange.EnteredPlayMode)
        {
            _isPlayMode = true;
        }
        else if (_isPlayMode && stateChange == PlayModeStateChange.ExitingPlayMode)
        {
            ResetData();
            _isPlayMode = false;
        }
    }
#endif

    public abstract void ResetData();
}
