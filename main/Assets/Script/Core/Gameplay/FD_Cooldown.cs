using UnityEngine;

namespace Assets.Script.Core.Gameplay
{
    public class FD_Cooldown
    {
        public float m_LastHit;
        public float m_Cooldown;

        public FD_Cooldown(float _cooldown)
        {
            m_LastHit = 0;
            m_Cooldown = _cooldown;
        }

        public void Reset()
        {
            m_LastHit = Time.fixedTime;
        }

        public bool IsElasped()
        {
            return m_LastHit + m_Cooldown < Time.fixedTime;
        }
    }
}
