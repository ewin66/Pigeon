﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

namespace Pigeon.Utils
{
    /// <summary>
    /// Represents an out-of-process asynchronous task to not block threads while waiting for results with automatic timeout
    /// </summary>
    /// <typeparam name="T">Task return type</typeparam>
    public struct RemoteTask<T>
    {
        private TaskCompletionSource<T> taskCompletionSource;
        private Timer timeoutTimer;

        private object lockObj;


        /// <summary>
        /// Initializes a new instance of <see cref="RemoteTask{T}"/>
        /// </summary>
        /// <param name="taskCompletionSource"><see cref="TaskCompletionSource{TResult}"/> that controls the result of the 
        /// <see cref="RemoteTask{T}"/></param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which the task will invoke the timeoutHandler</param>
        /// <param name="onTimeout">Function that is called on timeout, which can optionally return an exception to be raised on 
        /// the underlying task</param>
        public RemoteTask(TaskCompletionSource<T> taskCompletionSource, TimeSpan timeout, Func<Exception> onTimeout)
        {
            if (timeout.TotalMilliseconds < 0.0)
                throw new ArgumentException("Timeout requires a positive TimeSpan");

            if (onTimeout is null)
                throw new ArgumentNullException(nameof(onTimeout));

            this.taskCompletionSource = taskCompletionSource ?? throw new ArgumentNullException(nameof(taskCompletionSource));
            this.lockObj = new object();

            timeoutTimer = new Timer
            {
                Interval = Math.Max(timeout.TotalMilliseconds, 5.0), // Min timeout of 5ms
                AutoReset = false,
                Enabled = false
            };

            var at = this;

            timeoutTimer.Elapsed += (sender, args) =>
            {
                lock (at.lockObj)
                {
                    if (at.timeoutTimer is null)
                        return;

                    at.timeoutTimer.Stop();
                    var exception = onTimeout();
                    at.taskCompletionSource.TrySetException(exception ?? new TimeoutException());
                    at.taskCompletionSource = null;
                    at.timeoutTimer = null;
                    at.timeoutTimer = null;
                }
            };

            timeoutTimer.Enabled = true;
        }


        /// <summary>
        /// Transitions the underlying task to a completed status and returns the supplied result
        /// </summary>
        /// <param name="result">Result to return on the underlying task</param>
        public void CompleteWithResult(T result)
        {
            lock (lockObj)
            {
                if (timeoutTimer is null)
                    return;

                timeoutTimer.Stop();
                taskCompletionSource.TrySetResult(result);
                taskCompletionSource = null;
                timeoutTimer.Dispose();
                timeoutTimer = null;
            }
        }


        /// <summary>
        /// Transitions the underlying task to faulted status and throws the supplied <see cref="Exception"/>
        /// </summary>
        /// <param name="ex"><see cref="Exception"/> to throw</param>
        public void CompleteWithException(Exception ex)
        {
            lock (lockObj)
            {
                if (timeoutTimer is null)
                    return;

                timeoutTimer.Stop();

                MethodInfo preserveStackTrace = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
                preserveStackTrace?.Invoke(ex, null);
                taskCompletionSource.TrySetException(ex);

                taskCompletionSource = null;
                timeoutTimer.Dispose();
                timeoutTimer = null;
            }
        }
    }
}
