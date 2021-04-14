using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Signals are used to trigger events and pass data across multiple scenes. This allows for decoupling
    /// systems and reduced passing of object references.
    /// </summary>
    /// <typeparam name="T">Type of signal. Explicit typing is necessary to avoid boxing.</typeparam>
    public class Signal<T>
    {
        public delegate void SignalListener(T parameter);

        private readonly List<SignalListener> _listeners = new List<SignalListener>(1);

        /// <summary>
        /// Add a listener to this signal.
        /// </summary>
        /// <param name="listener">What is listening for this signal.</param>
        public void AddListener(SignalListener listener)
        {
            // Prevent adding duplicate listeners.
            Assert.IsTrue(!_listeners.Contains(listener));
            _listeners.Add(listener);
        }

        /// <summary>
        /// Remove a listener from this signal.
        /// </summary>
        /// <param name="listener">What is no longer listening for this signal.</param>
        public void RemoveListener(SignalListener listener)
        {
            _listeners.Remove(listener);
        }

        /// <summary>
        /// Dispatch this signal.
        /// </summary>
        /// <param name="parameter"></param>
        public void Dispatch(T parameter)
        {
            int listenersCount = _listeners.Count;
            for (var i = 0; i < listenersCount; ++i)
            {
                // Invoke the delegate.
                _listeners[i](parameter);
            }
        }
    }
}
