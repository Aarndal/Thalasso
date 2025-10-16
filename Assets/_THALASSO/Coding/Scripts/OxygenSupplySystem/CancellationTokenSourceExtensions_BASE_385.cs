using System;
using System.Threading;
using UnityEngine;

public static class CancellationTokenSourceExtensions
{
    public static bool TryCancel(this CancellationTokenSource cts)
    {
        if (cts is null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("CTS is null. Cannot be cancelled.");
#endif
            return false;
        }

        try
        {
            if (cts.IsCancellationRequested)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"CTS cancellation has already been requested: {cts}");
#endif
                return false;
            }
            cts.Cancel();
            return true;
        }
        catch (ObjectDisposedException ex)
        {
#if UNITY_EDITOR
            Debug.LogError($"CTS has already been disposed: {ex.Message}");
#endif
            return false;
        }
        catch (Exception ex)
        {
#if UNITY_EDITOR
            Debug.LogError($"Exception during CTS handling: {ex.Message}");
#endif
            return false;
        }

    }
}