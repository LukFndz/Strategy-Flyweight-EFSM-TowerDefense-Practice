using System.Collections.Generic;

namespace Assets.Scripts.Gameplay.Core.EFSM
{
    public class StateConfigurer<T>
    {
        #region Private Variables

        private State<T> _instance;
        private Dictionary<T, Transition<T>> _transitions = new Dictionary<T, Transition<T>>();

        #endregion

        #region Public Functions

        public StateConfigurer(State<T> state)
        {
            _instance = state;
        }

        /// <summary>
        /// Sets a transition from the current state to a target state.
        /// </summary>
        /// <param name="input">The input that triggers the transition.</param>
        /// <param name="target">The target state of the transition.</param>
        /// <returns>The current StateConfigurer instance.</returns>
        public StateConfigurer<T> SetTransition(T input, State<T> target)
        {
            _transitions.Add(input, new Transition<T>(input, target));
            return this;
        }

        /// <summary>
        /// Finalizes the configuration of the state with the defined transitions.
        /// </summary>
        public void Done()
        {
            _instance.Configure(_transitions);
        }

        #endregion
    }

    public static class StateConfigurer
    {
        #region Public Functions

        /// <summary>
        /// Creates a new StateConfigurer instance for a given state.
        /// </summary>
        /// <typeparam name="T">The type of input used for state transitions.</typeparam>
        /// <param name="state">The state to configure.</param>
        /// <returns>A new StateConfigurer instance.</returns>
        public static StateConfigurer<T> Create<T>(State<T> state)
        {
            return new StateConfigurer<T>(state);
        }

        #endregion
    }
}