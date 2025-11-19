using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.EFSM
{
    public class EventFSM<T>
    {
        #region Private Variables
        [Tooltip("Current state of the FSM")]
        private State<T> _current;
        #endregion

        #region Properties
        public State<T> Current { get { return _current; } }
        #endregion

        public EventFSM(State<T> initial)
        {
            _current = initial;
            _current.Enter(default(T));
        }

        #region Public Functions

        /// <summary>
        /// Sends input to the FSM to transition to a new state.
        /// </summary>
        /// <param name="input">The input that triggers the state transition.</param>
        public void SendInput(T input)
        {
            State<T> newState;

            if (_current.CheckInput(input, out newState))
            {
                _current.Exit(input);
                _current = newState;
                _current.Enter(input);
            }
        }

        /// <summary>
        /// Artificial Update of the current state.
        /// </summary>
        public void Update()
        {
            _current.Update();
        }

        /// <summary>
        /// Artificial Late update of the current state.
        /// </summary>
        public void LateUpdate()
        {
            _current.LateUpdate();
        }

        /// <summary>
        /// Artificial Fixed update of the current state.
        /// </summary>
        public void FixedUpdate()
        {
            _current.FixedUpdate();
        }
        #endregion
    }
}