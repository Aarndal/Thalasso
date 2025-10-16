using System;
using System.Threading;
using UnityEngine;

public static class CancellationTokenSourceExtensions
{
    public enum CTSOperationResult : sbyte
    {
        IsNull = -1,
        Failed = 0,
        Success = 1,
        AlreadyCancelled = 2,
        AlreadyDisposed = 3
    }

    private static readonly object _cancelDisposeLock = new();
    private static readonly object _cancelLock = new();
    private static readonly object _disposeLock = new();

    /// <summary>
    /// Attempts to cancel and dispose a CancellationTokenSource within a single atomic operation.
    /// </summary>
    /// <param name="cts">The CancellationTokenSource to process</param>
    /// <returns>The status of the operation</returns>
    public static CTSOperationResult TryCancelAndDispose(this CancellationTokenSource cts)
    {
        var cancelStatus = cts.TryCancel(_cancelDisposeLock);

        // If cancellation failed, do not attempt to dispose.
        if (cancelStatus != CTSOperationResult.Success && cancelStatus != CTSOperationResult.AlreadyCancelled)
        {
            return cancelStatus;
        }
        return cts.TryDispose(_cancelDisposeLock);
    }

    /// <summary>
    /// Attempts to cancel a CancellationTokenSource within a single atomic operation.
    /// </summary>
    /// <param name="cts">The CancellationTokenSource to process</param>
    /// <param name="lock">Optional lock object to use for synchronization. If null, a default lock will be used.</param>
    /// <returns>The status of the operation</returns>
    public static CTSOperationResult TryCancel(this CancellationTokenSource cts, object @lock = null)
    {
        if (cts is null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("CTS is null.");
#endif
            return CTSOperationResult.IsNull;
        }

        lock (@lock ??= _cancelLock)
        {
            try
            {
                if (cts.IsCancellationRequested)
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"CTS cancellation has already been requested: {cts}");
#endif
                    return CTSOperationResult.AlreadyCancelled;
                }

                cts.Cancel();

                return CTSOperationResult.Success;
            }
            catch (ObjectDisposedException ex)
            {
#if UNITY_EDITOR
                Debug.LogError($"CTS has already been disposed of: {ex.ObjectName}");
#endif
                return CTSOperationResult.AlreadyDisposed;
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Debug.LogError($"Exception during CTS handling: {ex.Message}");
#endif
                return CTSOperationResult.Failed;
            }
        }
    }

    /// <summary>
    /// Attempts to dispose of a CancellationTokenSource within a single atomic operation.
    /// </summary>
    /// <param name="cts">The CancellationTokenSource to process</param>
    /// <param name="lock">Optional lock object to use for synchronization. If null, a default lock will be used.</param>
    /// <returns>The status of the operation</returns>
    public static CTSOperationResult TryDispose(this CancellationTokenSource cts, object @lock = null)
    {
        if (cts is null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("CTS is null.");
#endif
            return CTSOperationResult.IsNull;
        }

        lock (@lock ??= _disposeLock)
        {
            try
            {
                cts.Dispose();
                return CTSOperationResult.Success;
            }
            catch (ObjectDisposedException ex)
            {
#if UNITY_EDITOR
                Debug.LogError($"CTS has already been disposed of: {ex.ObjectName}");
#endif
                return CTSOperationResult.AlreadyDisposed;
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Debug.LogError($"Exception during CTS disposal: {ex.Message}");
#endif
                return CTSOperationResult.Failed;
            }
        }
    }
}