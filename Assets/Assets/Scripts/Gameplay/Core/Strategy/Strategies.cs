using Assets.Scripts.Gameplay.Core;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Strategy
{
    public interface IStateStrategy<T>
    {
        void OnEnter(T entity);
        void OnUpdate(T entity);
        void OnExit(T entity);
    }


    public abstract class Strategy<T> : ScriptableObject, IStateStrategy<T>
    {
        public virtual void OnEnter(T entity) { }
        public virtual void OnExit(T entity) { }
        public virtual void OnUpdate(T entity) { }

    }
}