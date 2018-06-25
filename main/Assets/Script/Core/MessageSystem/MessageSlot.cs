using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Core.MessageSystem
{
    public class MessageSlot
    {
        private event MessageManager.OnMessage m_OnMessageEvent;

        public void Dispatch(IMessage _msg)
        {
            if (m_OnMessageEvent != null)
            {
                m_OnMessageEvent(_msg);
            }
        }

        public static MessageSlot operator +(MessageSlot _slot, MessageManager.OnMessage _delegate)
        {
            _slot.m_OnMessageEvent += _delegate;
            return _slot;
        }

        public static MessageSlot operator -(MessageSlot _slot, MessageManager.OnMessage _delegate)
        {
            _slot.m_OnMessageEvent -= _delegate;
            return _slot;
        }
    }
}
