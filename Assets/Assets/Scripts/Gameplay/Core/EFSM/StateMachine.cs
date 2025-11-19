using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.EFSM
{
    /// <summary>
    /// Interface to define common behaviors for any entity with an FSM.
    /// </summary>
    public interface IStateEntity<T>
    {
        /// <summary>
        /// Configure the FSM transition, strategies, and states.
        /// </summary>
        void ConfigureFSM();
        void SendInputToFSM(T inp);
    }

    public enum EnemyStates { Walking, Attack }

    public enum TowerStates { Idle, Attack }

    public class StateMachine<T> : MonoBehaviour, IStateEntity<T> where T : System.Enum
    {
        #region Private Variables

        protected EventFSM<T> _fsm;

        #endregion

        #region Public Functions

        /// <summary>
        /// Configures the FSM. This method should be overridden.
        /// </summary>
        public virtual void ConfigureFSM()
        {
        }

        /// <summary>
        /// Sends an input to the FSM.
        /// </summary>
        /// <param name="inp">The input to send.</param>
        public void SendInputToFSM(T inp)
        {
            _fsm.SendInput(inp);
        }

        #endregion

        #region Monobehaviour Functions

        private void Update()
        {
            _fsm?.Update();
        }

        #endregion
    }
}