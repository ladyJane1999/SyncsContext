using System.Collections.Concurrent;

namespace SyncsContext
{
    sealed class SingleThreadSynchronizationContext :
             SynchronizationContext
    {
        readonly CancellationToken _cancellationToken;
        readonly BlockingCollection<Tuple<SendOrPostCallback, object>> _queue;
        bool _completed;
        private readonly Thread _thread;
        public SingleThreadSynchronizationContext(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _queue = new BlockingCollection<Tuple<SendOrPostCallback, object>>();
            _thread = new Thread(RunOnCurrentThread);
            _thread.Start(this);
        }
        public override void Post(SendOrPostCallback callback, object state)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            if (_completed)
                throw new TaskSchedulerException("The synchronization context was already completed");
            try
            {
                _queue.Add(Tuple.Create(callback, state), _cancellationToken);
            }
            catch (InvalidOperationException)
            {
            }
        }
        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException("Synchronously sending is not supported.");
        }
        public void RunOnCurrentThread(object obj)
        {
            SetSynchronizationContext(obj as SynchronizationContext);
            CancellationToken cancellationToken = new CancellationToken();
            Tuple<SendOrPostCallback, object> callback;
            while (_queue.TryTake(out callback, 50, cancellationToken))
                callback.Item1(callback.Item2);
        }

        public void SetComplete()
        {
            _completed = true;
        }
    }
}

