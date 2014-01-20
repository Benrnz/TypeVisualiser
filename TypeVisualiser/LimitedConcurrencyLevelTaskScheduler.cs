namespace TypeVisualiser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a task scheduler that ensures a maximum concurrency level while
    /// running on top of the ThreadPool.
    /// Source: http://msdn.microsoft.com/en-us/library/ee789351.aspx
    /// Author: Microsoft 2009
    /// </summary>
    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        /// <summary>
        /// The maximum concurrency level allowed by this scheduler.
        /// </summary>
        private readonly int maxDegreeOfParallelism;

        /// <summary>
        /// The list of tasks to be executed.
        /// </summary>
        private readonly LinkedList<Task> tasks = new LinkedList<Task>();

        /// <summary>
        /// Whether the current thread is processing work items.
        /// </summary>
        [ThreadStatic]
        private static bool currentThreadIsProcessingItems;

        /// <summary>
        /// Whether the scheduler is currently processing work items.
        /// </summary>
        private int delegatesQueuedOrRunning;

        /// <summary>
        /// Initializes a new instance of the LimitedConcurrencyLevelTaskScheduler class with the
        /// specified degree of parallelism.
        /// </summary>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param>
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;

            if (this.maxDegreeOfParallelism < 1)
            {
                throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
            }
        }

        /// <summary>
        /// Gets the maximum concurrency level supported by this scheduler.
        /// </summary>
        public sealed override int MaximumConcurrencyLevel
        {
            get { return this.maxDegreeOfParallelism; }
        }

        /// <summary>
        /// Queues a task to the scheduler.
        /// </summary>
        /// <param name="task">The task to be queued.</param>
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough
            // delegates currently queued or running to process tasks, schedule another.
            lock (this.tasks)
            {
                this.tasks.AddLast(task);
                if (this.delegatesQueuedOrRunning < this.maxDegreeOfParallelism)
                {
                    ++this.delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        /// <summary>
        /// Attempts to execute the specified task on the current thread.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">A Boolean denoting whether or not task has previously been queued. If this parameter is True, then the task may have been previously queued (scheduled); if False, then the task is known not to have been queued, and this call is being made in order to execute the task inline without queuing it.</param>
        /// <returns>
        /// Whether the task could be executed on the current thread.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="task"/> argument is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The <paramref name="task"/> was already executed.</exception>
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!currentThreadIsProcessingItems)
            {
                return false;
            }

            // If the task was previously queued, remove it from the queue
            if (taskWasPreviouslyQueued)
            {
                TryDequeue(task);
            }

            // Try to run the task.
            return TryExecuteTask(task);
        }

        /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
        /// <param name="task">The task to be removed.</param>
        /// <returns>Whether the task could be found and removed.</returns>
        protected sealed override bool TryDequeue(Task task)
        {
            lock (this.tasks) return this.tasks.Remove(task);
        }

        /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary>
        /// <returns>An enumerable of the tasks currently scheduled.</returns>
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(this.tasks, ref lockTaken);
                if (lockTaken)
                {
                    return this.tasks.ToArray();
                } else
                {
                    throw new NotSupportedException();
                }
            } finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(this.tasks);
                }
            }
        }

        /// <summary>
        /// Informs the ThreadPool that there's work to be executed for this scheduler.
        /// </summary>
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(
                _ =>
                {
                    // Note that the current thread is now processing work items.
                    // This is necessary to enable inlining of tasks into this thread.
                    currentThreadIsProcessingItems = true;
                    try
                    {
                        // Process all available items in the queue.
                        while (true)
                        {
                            Task item;
                            lock (this.tasks)
                            {
                                // When there are no more items to be processed,
                                // note that we're done processing, and get out.
                                if (this.tasks.Count == 0)
                                {
                                    --this.delegatesQueuedOrRunning;
                                    break;
                                }

                                // Get the next item from the queue
                                item = this.tasks.First.Value;
                                this.tasks.RemoveFirst();
                            }

                            // Execute the task we pulled out of the queue
                            TryExecuteTask(item);
                        }
                    } finally
                    {
                        // We're done processing items on the current thread
                        currentThreadIsProcessingItems = false;
                    }
                },
            null);
        }
    }
}
