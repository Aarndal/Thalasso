using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MySettingsManager
{
    public static readonly Dictionary<string, Canvas> Canvases = new();
    public static readonly Dictionary<string, IAmSettable> Settings = new();

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
        foreach (object arg in eventArgs)
        {
            if (arg is string canvasName && Canvases.Keys.Contains(canvasName))
                PlayerPrefs.Save();
        }
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
