using System;
using System.Collections;
using UnityEngine;

public class QuitGameButton : ButtonClick
{
    [SerializeField, Range(0.0f, 1.0f)]
    private float _secondsTillShutDown = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        Application.wantsToQuit += OnWantsToQuit;
    }

    protected void OnDestroy()
    {
        Application.wantsToQuit -= OnWantsToQuit;
    }

    protected override void OnClicked()
    {
        base.OnClicked();

        GlobalEventBus.Raise(GlobalEvents.UI.MenuClosed, "MainMenuCanvas");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private bool OnWantsToQuit()
    {
        StartCoroutine(DelayShutDown());
        return true;
    }

    private IEnumerator DelayShutDown()
    {
        yield return new WaitForSeconds(_secondsTillShutDown);
    }
}
