using System;

namespace Assets.Scripts.Gameplay.Core.EFSM
{
	public class Transition<T>
    {

        #region Private Variables
        T input;
        State<T> targetState;
        #endregion

        #region Properties
        public event Action<T> OnTransition = delegate { };
		public T Input { get { return input; } }
		public State<T> TargetState { get { return targetState;  } }
        #endregion

        public Transition(T input, State<T> targetState)
        {
            this.input = input;
            this.targetState = targetState;
        }

        /// <summary>
        /// Executes the transition action for the given input.
        /// </summary>
        /// <param name="input">Input that triggers the transition.</param>
        public void OnTransitionExecute(T input)
        {
			OnTransition(input);
		}
	}
}