using UnityEngine;
using System;
using System.Collections.Generic;

#pragma warning disable 649

namespace Assets.Script.Core.StateMachine
{
    public class MachineBehavior : MonoBehaviour 
    {
        [SerializeField]
        State[] m_States;

        [SerializeField]
        State m_InitialState;

        
        Dictionary<Enum, State> m_StatesMap;
        Enum m_CurrentStateId;
        Enum m_DeferredStateId;
        State m_CurrentState;

        void Awake()
        {
            m_StatesMap = new Dictionary<Enum, State>();
            foreach (State newState in m_States)
            {
                m_StatesMap.Add(newState.GetId(), newState);
                newState.Init(this);
            }

            m_DeferredStateId = m_InitialState.GetId();
            m_CurrentStateId = m_InitialState.GetId();
            m_CurrentState = m_StatesMap[m_CurrentStateId];
        }

        void OnEnable()
        {
            foreach (State state in m_StatesMap.Values)
                state.OnGameObjectEnable();
        }

        void OnDisable()
        {
            foreach (State state in m_StatesMap.Values)
                state.OnGameObjectDisable();
        }

        void Start()
        {
            m_CurrentState.OnEnter();
        }

	    void FixedUpdate () 
        {
	        if(m_DeferredStateId != m_CurrentStateId)
            {
                m_CurrentState.OnExit();

                m_CurrentStateId = m_DeferredStateId;
                m_CurrentState = m_StatesMap[m_CurrentStateId];

                m_CurrentState.OnEnter();
            }

            m_CurrentState.OnFixedUpdate();
	    }

        void LateUpdate()
        {
            m_CurrentState.OnLateUpdate();
        }

        public void ChangeState(Enum _stateId)
        {
            m_DeferredStateId = _stateId;
        }
    }
}