using System;
using UnityEngine;

public static class MySettingsManager
{
    static MySettingsManager()
    {
        GlobalEventBus.Register(GlobalEvents.UI.CanvasDisabled, OnCanvasDisabled);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        Application.quitting += OnQuitting;
        Application.wantsToQuit += OnWantsToQuit;
    }

    private static void OnCanvasDisabled(object[] eventArgs)
    {
        PlayerPrefs.Save();
    }

    private static void OnQuitting()
    {
        PlayerPrefs.Save();
    }

    private static bool OnWantsToQuit()
    {
        PlayerPrefs.Save();
        return true;
    }
}
