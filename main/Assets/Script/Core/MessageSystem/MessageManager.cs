using System;
using System.Collections.Generic;

namespace Assets.Script.Core.MessageSystem
{
    public class MessageManager
    {
        private static MessageManager m_Instance = new MessageManager();

        public delegate void OnMessage(IMessage _msg);

        private Dictionary<Enum, MessageSlot> m_Slots;

        public static MessageManager Instance
        {
            get { return m_Instance; }
        }

        private MessageManager()
        {
            m_Slots = new Dictionary<Enum, MessageSlot>();
        }

        public void Connect(Enum _id, OnMessage _delegate)
        {
            if (!m_Slots.ContainsKey(_id))
                m_Slots[_id] = new MessageSlot();

            m_Slots[_id] += _delegate;
        }

        public void Detach(Enum _id, OnMessage _delegate)
        {
            if (!m_Slots.ContainsKey(_id))
                return;

            m_Slots[_id] -= _delegate;
        }

        public void Send(IMessage _msg)
        {
            MessageSlot slot;
            if (!m_Slots.TryGetValue(_msg.Id, out slot))
            {

#if !UNITY_EDITOR
                UnityEngine.Debug.LogError(string.Format("You send the message {0} but no one listen to it!", _msg.Id));
#endif
                return;
            }

            slot.Dispatch(_msg);
        }

        public void Send(Enum _id)
        {
            MessageSlot slot;
            if (!m_Slots.TryGetValue(_id, out slot))
            {
#if !UNITY_EDITOR
                UnityEngine.Debug.LogError(string.Format("You send the message {0} but no one listen to it!", _id));
#endif
                return;
            }

            slot.Dispatch(null);
        }
    }
}
