using System;
using System.Collections.Generic;

namespace Assets.Scripts.Gameplay.Core.EFSM
{
    public class State<T>
    {
        #region Properties

        public string Name => _stateName;

        #endregion

        #region Events

        public event Action<T> OnEnter = delegate { };
        public event Action OnUpdate = delegate { };
        public event Action OnLateUpdate = delegate { };
        public event Action OnFixedUpdate = delegate { };
        public event Action<T> OnExit = delegate { };

        #endregion

        #region Private Variables

        private string _stateName;
        private Dictionary<T, Transition<T>> _transitions;

        #endregion

        public State(string name)
        {
            _stateName = name;
        }

        #region Public Functions

        /// <summary>
        /// Configures the state with its transitions.
        /// </summary>
        /// <param name="transitions">Dictionary of transitions.</param>
        public State<T> Configure(Dictionary<T, Transition<T>> transitions)
        {
            _transitions = transitions;
            return this;
        }

        /// <summary>
        /// Retrieves the transition for the given input.
        /// </summary>
        /// <param name="input">Input triggering the transition.</param>
        /// <returns>The corresponding transition.</returns>
        public Transition<T> GetTransition(T input)
        {
            return _transitions[input];
        }

        /// <summary>
        /// Checks if there is a valid transition for the given input.
        /// </summary>
        /// <param name="input">Input to check.</param>
        /// <param name="next">Next state if transition is valid.</param>
        /// <returns>True if a transition exists, otherwise false.</returns>
        public bool CheckInput(T input, out State<T> next)
        {
            if (_transitions.ContainsKey(input))
            {
                var transition = _transitions[input];
                transition.OnTransitionExecute(input);
                next = transition.TargetState;
                return true;
            }

            next = this;
            return false;
        }

        /// <summary>
        /// Executes the OnEnter event for the state.
        /// </summary>
        /// <param name="input">Input triggering the enter event.</param>
        public void Enter(T input)
        {
            OnEnter(input);
        }

        /// <summary>
        /// Executes the OnUpdate event for the state.
        /// </summary>
        public void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// Executes the OnLateUpdate event for the state.
        /// </summary>
        public void LateUpdate()
        {
            OnLateUpdate();
        }

        /// <summary>
        /// Executes the OnFixedUpdate event for the state.
        /// </summary>
        public void FixedUpdate()
        {
            OnFixedUpdate();
        }

        /// <summary>
        /// Executes the OnExit event for the state.
        /// </summary>
        /// <param name="input">Input triggering the exit event.</param>
        public void Exit(T input)
        {
            OnExit(input);
        }

        #endregion
    }
}
