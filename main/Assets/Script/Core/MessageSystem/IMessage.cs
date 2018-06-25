using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Core.MessageSystem
{
    public class IMessage
    {
        private Enum m_Id;

        public Enum Id
        {
            get { return m_Id; }
        }

        public IMessage(Enum _Id)
        {
            m_Id = _Id;
        }

        public T GetMessage<T>()
            where T : IMessage
        {
            return this as T;
        }
    }
}
