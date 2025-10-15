using System;
using System.Threading;
using UnityEngine;


/// <summary>
/// Extension methods for safer handling of CancellationTokenSource in Unity with UniTask.
/// </summary>
public static class CancellationTokenSourceExtensions
{
    /// <summary>
    /// Represents the status of operations on CancellationTokenSource.
    /// </summary>
    public enum OperationStatus : sbyte
    {
        /// <summary>The operation failed with an exception.</summary>
        Failed = -3,
        /// <summary>The operation was interrupted.</summary>
        Interrupted = -2,
        /// <summary>The token cannot be cancelled.</summary>
        CannotBeCancelled = -1,
        /// <summary>The CancellationTokenSource is null.</summary>
        IsNull = 0,
        /// <summary>The operation was successful.</summary>
        Success = 1,
        /// <summary>The CancellationTokenSource was already cancelled.</summary>
        AlreadyCancelled = 2,
        /// <summary>The CancellationTokenSource was already disposed.</summary>
        AlreadyDisposed = 3,
    }


    /// <summary>
    /// Result information for debugging and detailed error reporting
    /// </summary>
    public struct OperationResult
    {
        public OperationStatus Status { get; internal set; }
        public Exception Exception { get; internal set; }
        public string Message { get; internal set; }

        public override readonly string ToString() =>
            $"{Status}: {Message}{(Exception != null ? $" - {Exception.GetType().Name}: {Exception.Message}" : "")}";
    }


    #region Private Members

    // Internal lock objects with explicit naming to clarify their purpose
    private static readonly object _defaultCancelLock = new();
    private static readonly object _defaultDisposeLock = new();
    private static readonly object _defaultCombinedLock = new();

    #endregion


    #region Public Members

    /// <summary>
    /// Attempts to cancel and dispose a CancellationTokenSource safely.
    /// </summary>
    /// <param name="cts">The CancellationTokenSource to cancel and dispose.</param>
    /// <param name="result">Detailed result of the operation.</param>
    /// <param name="lock">Optional custom lock object to synchronize access.</param>
    /// <returns>True if both cancel and dispose operations succeeded; otherwise false.</returns>
    /// <remarks>
    /// If you provide a custom lock object, the same lock will be used for both cancel and dispose operations.
    /// This method will not attempt to dispose if cancellation fails with specific error conditions.
    /// </remarks>
    public static bool TryCancelAndDispose(this CancellationTokenSource cts, out OperationResult result, object @lock = null)
    {
        object effectiveLock = @lock ?? _defaultCombinedLock;

        result = new OperationResult();

        // Try to cancel first
        bool cancelSuccess = cts.TryCancel(out var cancelResult, effectiveLock);

        // If cancellation failed with a fatal error, don't attempt to dispose
        if (!cancelSuccess && (
            cancelResult.Status == OperationStatus.IsNull ||
            cancelResult.Status == OperationStatus.AlreadyDisposed))
        {
            result.Status = cancelResult.Status;
            result.Message = $"Cancel failed: {cancelResult.Message}";
            result.Exception = cancelResult.Exception;
            return false;
        }

        // Try to dispose
        bool disposeSuccess = cts.TryDispose(out var disposeResult, effectiveLock);

        // Set the final result based on both operations
        if (cancelSuccess && disposeSuccess)
        {
            result.Status = OperationStatus.Success;
            result.Message = "Successfully cancelled and disposed";
            return true;
        }
        else
        {
            // Report the first failure
            if (!cancelSuccess)
            {
                result.Status = cancelResult.Status;
                result.Message = $"Cancel failed: {cancelResult.Message}" +
                    (disposeSuccess ? ", but dispose succeeded" : $", and dispose also failed: {disposeResult.Message}");
                result.Exception = cancelResult.Exception;
            }
            else
            {
                result.Status = disposeResult.Status;
                result.Message = $"Cancel succeeded, but dispose failed: {disposeResult.Message}";
                result.Exception = disposeResult.Exception;
            }
            return false;
        }
    }

    /// <summary>
    /// Attempts to cancel and dispose a CancellationTokenSource safely with simplified result.
    /// </summary>
    /// <param name="cts">The CancellationTokenSource to cancel and dispose.</param>
    /// <param name="lock">Optional custom lock object to synchronize access.</param>
    /// <returns>True if both cancel and dispose operations succeeded; otherwise false.</returns>
    public static bool TryCancelAndDispose(this CancellationTokenSource cts, object @lock = null)
    {
        return cts.TryCancelAndDispose(out var _, @lock);
    }

    /// <summary>
    /// Attempts to cancel a CancellationTokenSource safely.
    /// </summary>
    /// <param name="cts">The CancellationTokenSource to cancel.</param>
    /// <param name="result">Detailed result of the operation.</param>
    /// <param name="lock">Optional custom lock object to synchronize access.</param>
    /// <returns>True if cancellation was successful; otherwise false.</returns>
    public static bool TryCancel(this CancellationTokenSource cts, out OperationResult result, object @lock = null)
    {
        result = new OperationResult();

        // Check for null outside the lock to fail fast
        if (cts == null)
        {
            LogWarning("CTS is null.");
            result.Status = OperationStatus.IsNull;
            result.Message = "CancellationTokenSource is null";
            return false;
        }

        object effectiveLock = @lock ?? _defaultCancelLock;

        try
        {
            lock (effectiveLock)
            {
                try
                {
                    // Check if already cancelled
                    if (cts.IsCancellationRequested)
                    {
                        LogWarning($"CTS cancellation has already been requested: {cts}");
                        result.Status = OperationStatus.AlreadyCancelled;
                        result.Message = "Cancellation has already been requested";
                        return false;
                    }

                    // Check if token can be cancelled
                    if (!cts.Token.CanBeCanceled)
                    {
                        LogWarning($"CTS Token cannot be cancelled: {cts.Token}");
                        result.Status = OperationStatus.CannotBeCancelled;
                        result.Message = "Token cannot be cancelled";
                        return false;
                    }

                    // Perform cancellation
                    cts.Cancel();
                    result.Status = OperationStatus.Success;
                    result.Message = "Successfully cancelled";
                    return true;
                }
                catch (ObjectDisposedException ex)
                {
                    LogError($"CTS has already been disposed: {ex.ObjectName}");
                    result.Status = OperationStatus.AlreadyDisposed;
                    result.Message = $"Already disposed: {ex.ObjectName}";
                    result.Exception = ex;
                    return false;
                }
                catch (NullReferenceException ex)
                {
                    LogError($"NullReferenceException during CTS cancellation: {ex.Message}");
                    result.Status = OperationStatus.IsNull;
                    result.Message = $"Null reference during cancellation";
                    result.Exception = ex;
                    return false;
                }
                catch (Exception ex)
                {
                    LogError($"Exception during CTS cancellation: {ex.Message}");
                    result.Status = OperationStatus.Failed;
                    result.Message = $"General failure during cancellation";
                    result.Exception = ex;
                    return false;
                }
            }
        }
        catch (ThreadInterruptedException ex)
        {
            LogError($"Thread was interrupted while trying to cancel CTS: {ex.Message}");
            result.Status = OperationStatus.Interrupted;
            result.Message = $"Thread interrupted during lock acquisition";
            result.Exception = ex;
            return false;
        }
    }

    /// <summary>
    /// Attempts to cancel a CancellationTokenSource safely with simplified result.
    /// </summary>
    /// <param name="cts">The CancellationTokenSource to cancel.</param>
    /// <param name="lock">Optional custom lock object to synchronize access.</param>
    /// <returns>True if cancellation was successful; otherwise false.</returns>
    public static bool TryCancel(this CancellationTokenSource cts, object @lock = null)
    {
        return cts.TryCancel(out var _, @lock);
    }

    /// <summary>
    /// Attempts to dispose a CancellationTokenSource safely.
    /// </summary>
    /// <param name="cts">The CancellationTokenSource to dispose.</param>
    /// <param name="result">Detailed result of the operation.</param>
    /// <param name="lock">Optional custom lock object to synchronize access.</param>
    /// <returns>True if disposal was successful; otherwise false.</returns>
    public static bool TryDispose(this CancellationTokenSource cts, out OperationResult result, object @lock = null)
    {
        result = new OperationResult();

        // Check for null outside the lock to fail fast
        if (cts == null)
        {
            LogWarning("CTS is null.");
            result.Status = OperationStatus.IsNull;
            result.Message = "CancellationTokenSource is null";
            return false;
        }

        object effectiveLock = @lock ?? _defaultDisposeLock;

        try
        {
            lock (effectiveLock)
            {
                try
                {
                    // Perform disposal
                    cts.Dispose();
                    result.Status = OperationStatus.Success;
                    result.Message = "Successfully disposed";
                    return true;
                }
                catch (ObjectDisposedException ex)
                {
                    LogError($"CTS has already been disposed: {ex.ObjectName}");
                    result.Status = OperationStatus.AlreadyDisposed;
                    result.Message = $"Already disposed: {ex.ObjectName}";
                    result.Exception = ex;
                    return false;
                }
                catch (NullReferenceException ex)
                {
                    LogError($"NullReferenceException during CTS disposal: {ex.Message}");
                    result.Status = OperationStatus.IsNull;
                    result.Message = "Null reference during disposal";
                    result.Exception = ex;
                    return false;
                }
                catch (Exception ex)
                {
                    LogError($"Exception during CTS disposal: {ex.Message}");
                    result.Status = OperationStatus.Failed;
                    result.Message = "General failure during disposal";
                    result.Exception = ex;
                    return false;
                }
            }
        }
        catch (ThreadInterruptedException ex)
        {
            LogError($"Thread was interrupted while trying to dispose CTS: {ex.Message}");
            result.Status = OperationStatus.Interrupted;
            result.Message = "Thread interrupted during lock acquisition";
            result.Exception = ex;
            return false;
        }
    }

    /// <summary>
    /// Attempts to dispose a CancellationTokenSource safely with simplified result.
    /// </summary>
    /// <param name="cts">The CancellationTokenSource to dispose.</param>
    /// <param name="lock">Optional custom lock object to synchronize access.</param>
    /// <returns>True if disposal was successful; otherwise false.</returns>
    public static bool TryDispose(this CancellationTokenSource cts, object @lock = null)
    {
        return cts.TryDispose(out var _, @lock);
    }

    #endregion


    #region Private Members

    // Logging helpers with conditional compilation
#if UNITY_EDITOR
    private static void LogWarning(string message) => Debug.LogWarning(message);
    private static void LogError(string message) => Debug.LogError(message);
#else
    private static void LogWarning(string message)
    {
        // No logging in non-editor builds or implement alternative logging here
    }

    private static void LogError(string message)
    {
        // No logging in non-editor builds or implement alternative logging here
    }
#endif

    #endregion
}
