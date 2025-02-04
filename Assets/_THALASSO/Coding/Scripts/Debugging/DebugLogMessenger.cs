using UnityEngine;

public static class DebugLogMessenger
{
    static DebugLogMessenger() => Debug.unityLogger.logEnabled = true;

}
