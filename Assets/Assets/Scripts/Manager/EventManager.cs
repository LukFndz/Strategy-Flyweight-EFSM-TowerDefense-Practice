using System;
using System.Collections.Generic;

namespace Assets.Scripts.Managers
{
    public class EventManager : SingletonMonoBehaviour<EventManager>
    {
        #region Private Variables

        private Dictionary<EventName, Action<object[]>> _subscribers = new();

        #endregion

        #region Public Methods
        /// <summary>
        /// Subscribe to an event
        /// </summary>
        /// <param name="eventId"> The event to subscribe to </param>
        /// <param name="callback"> The function to call when the event is triggered </param>
        public void Subscribe(EventName eventId, Action<object[]> callback)
        {
            if (!_subscribers.TryAdd(eventId, callback))
            {
                _subscribers[eventId] += callback;
            }
        }

        /// <summary>
        /// Unsubscribe from an event
        /// </summary>
        /// <param name="eventId"> The event to unsubscribe from </param>
        /// <param name="callback"> The function to unsubscribe </param>
        public void Unsubscribe(EventName eventId, Action<object[]> callback)
        {
            if (!_subscribers.ContainsKey(eventId)) { return; }

            _subscribers[eventId] -= callback;
        }

        /// <summary>
        /// Trigger an event
        /// </summary>
        /// <param name="eventId"> The event to trigger </param>
        /// <param name="parameters"> The parameters to pass to the event </param>
        public void Trigger(EventName eventId, params object[] parameters)
        {

            if (!_subscribers.ContainsKey(eventId))
            {
                return;
            }

            _subscribers[eventId]?.Invoke(parameters);
        }
        #endregion
    }

    /// <summary>
    ///  The events that can be triggered
    /// </summary>
    public enum EventName
    {
        CardsLoaded,
        HandUpdate,
        CardDrag,
        CardEndDrag,
        CardUpdate,
    }
}