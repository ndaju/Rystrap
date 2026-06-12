using System.Collections.Concurrent;

using Rystrap.Plugins.Events;

namespace Rystrap.Plugins
{
    /// <summary>
    /// Thread-safe event bus implementation for plugin events.
    /// Supports both synchronous and asynchronous handlers.
    /// </summary>
    internal sealed class PluginEventBus : IPluginEventBus
    {
        private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();
        private readonly object _lock = new();

        /// <summary>
        /// Subscribe to an event with a synchronous handler.
        /// </summary>
        public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IPluginEvent
        {
            return AddHandler<TEvent>(handler);
        }

        /// <summary>
        /// Subscribe to an event with an asynchronous handler.
        /// </summary>
        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IPluginEvent
        {
            return AddHandler<TEvent>(handler);
        }

        /// <summary>
        /// Publish an event asynchronously to all subscribers.
        /// </summary>
        public async Task PublishAsync<TEvent>(TEvent eventData) where TEvent : IPluginEvent
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
                return;

            List<Delegate> handlersCopy;
            lock (_lock)
            {
                handlersCopy = new List<Delegate>(handlers);
            }

            foreach (var handler in handlersCopy)
            {
                try
                {
                    if (handler is Action<TEvent> syncHandler)
                    {
                        syncHandler(eventData);
                    }
                    else if (handler is Func<TEvent, Task> asyncHandler)
                    {
                        await asyncHandler(eventData).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException($"PluginEventBus::PublishAsync<{typeof(TEvent).Name}>", ex);
                }
            }
        }

        /// <summary>
        /// Publish an event synchronously to all subscribers.
        /// </summary>
        public void Publish<TEvent>(TEvent eventData) where TEvent : IPluginEvent
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
                return;

            List<Delegate> handlersCopy;
            lock (_lock)
            {
                handlersCopy = new List<Delegate>(handlers);
            }

            foreach (var handler in handlersCopy)
            {
                try
                {
                    if (handler is Action<TEvent> syncHandler)
                    {
                        syncHandler(eventData);
                    }
                    else if (handler is Func<TEvent, Task> asyncHandler)
                    {
                        // Run async handler synchronously (blocking)
                        asyncHandler(eventData).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException($"PluginEventBus::Publish<{typeof(TEvent).Name}>", ex);
                }
            }
        }

        /// <summary>
        /// Remove all handlers for a specific event type.
        /// </summary>
        public void ClearHandlers<TEvent>() where TEvent : IPluginEvent
        {
            _handlers.TryRemove(typeof(TEvent), out _);
        }

        /// <summary>
        /// Remove all handlers for all event types.
        /// </summary>
        public void ClearAll()
        {
            _handlers.Clear();
        }

        private IDisposable AddHandler<TEvent>(Delegate handler) where TEvent : IPluginEvent
        {
            var eventType = typeof(TEvent);

            lock (_lock)
            {
                var handlers = _handlers.GetOrAdd(eventType, _ => new List<Delegate>());
                handlers.Add(handler);
            }

            return new Subscription(() => RemoveHandler<TEvent>(handler));
        }

        private void RemoveHandler<TEvent>(Delegate handler) where TEvent : IPluginEvent
        {
            lock (_lock)
            {
                if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
                {
                    handlers.Remove(handler);

                    if (handlers.Count == 0)
                    {
                        _handlers.TryRemove(typeof(TEvent), out _);
                    }
                }
            }
        }

        private sealed class Subscription : IDisposable
        {
            private Action? _removeAction;
            private bool _disposed;

            public Subscription(Action removeAction)
            {
                _removeAction = removeAction;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _removeAction?.Invoke();
                    _removeAction = null;
                    _disposed = true;
                }
            }
        }
    }
}
