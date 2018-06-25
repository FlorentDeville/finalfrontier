using System;
using UnityEngine;

namespace Assets.Script.Core.StateMachine
{
    /// <summary>
    /// Order of event:
    ///  - Init
    ///  - OnEnable
    ///  
    ///  - OnExit //if necessary
    ///  - OnEnter //if necessary
    ///  - OnFixedUpdate
    ///  - OnLateUpdate
    /// </summary>
    [Serializable]
    public abstract class State : MonoBehaviour
    {
        protected MachineBehavior m_Machine;

        public State() { }

        public virtual void Init(MachineBehavior _machine)
        {
            m_Machine = _machine;
        }

        public virtual void OnGameObjectEnable() { }
        public virtual void OnGameObjectDisable() { }

        public virtual void OnEnter() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnLateUpdate() { }
        public virtual void OnExit() { }
        
        public abstract Enum GetId();
    }
}
