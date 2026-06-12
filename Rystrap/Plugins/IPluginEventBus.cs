using Rystrap.Plugins.Events;

namespace Rystrap.Plugins
{
    /// <summary>
    /// Event bus for plugins to subscribe to and publish events.
    /// Thread-safe implementation for concurrent plugin operations.
    /// </summary>
    public interface IPluginEventBus
    {
        /// <summary>
        /// Subscribe to an event with a synchronous handler.
        /// </summary>
        /// <typeparam name="TEvent">Event type to subscribe to.</typeparam>
        /// <param name="handler">Handler to invoke when event is published.</param>
        /// <returns>Disposable subscription that can be used to unsubscribe.</returns>
        IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IPluginEvent;

        /// <summary>
        /// Subscribe to an event with an asynchronous handler.
        /// </summary>
        /// <typeparam name="TEvent">Event type to subscribe to.</typeparam>
        /// <param name="handler">Async handler to invoke when event is published.</param>
        /// <returns>Disposable subscription that can be used to unsubscribe.</returns>
        IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IPluginEvent;

        /// <summary>
        /// Publish an event to all subscribers.
        /// </summary>
        /// <typeparam name="TEvent">Event type to publish.</typeparam>
        /// <param name="eventData">Event data to publish.</param>
        /// <returns>Task representing the publish operation.</returns>
        Task PublishAsync<TEvent>(TEvent eventData) where TEvent : IPluginEvent;

        /// <summary>
        /// Publish an event synchronously (waits for all handlers to complete).
        /// </summary>
        /// <typeparam name="TEvent">Event type to publish.</typeparam>
        /// <param name="eventData">Event data to publish.</param>
        void Publish<TEvent>(TEvent eventData) where TEvent : IPluginEvent;
    }
}
