using System;

namespace Sibusten.Philomena.Client.Utilities
{
    /// <summary>
    /// An <see cref="IProgress{T}"/> implementation that posts progress updates synchronously. This allows for in-order progress updates in console apps which don't have a synchronization context.
    /// </summary>
    /// <typeparam name="TProgress">The progress update type</typeparam>
    public class SyncProgress<TProgress> : IProgress<TProgress>
    {
        private readonly Action<TProgress>? _handler;

        public event EventHandler<TProgress>? ProgressChanged;

        public SyncProgress() { }

        public SyncProgress(Action<TProgress> handler)
        {
            _handler = handler;
        }

        public void Report(TProgress value)
        {
            // Synchronously invoke the progress callbacks
            _handler?.Invoke(value);
            ProgressChanged?.Invoke(this, value);
        }
    }
}
