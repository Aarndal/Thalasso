using System;
using System.Collections;
using UnityEngine;

public class QuitGameButton : ButtonClick
{
    [SerializeField, Range(0.0f, 1.0f)]
    private float _secondsTillShutDown = 0.0f;
    protected override void OnClicked()
    {
        base.OnClicked();

        StartCoroutine(DelayShutDown());

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private IEnumerator DelayShutDown()
    {
        yield return new WaitForSeconds(_secondsTillShutDown);
    }
}
