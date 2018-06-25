using System;
using UnityEngine;

namespace Assets.Script.Game.Compounds
{
    public class CompoundManager : MonoBehaviour
    {
        private static CompoundManager m_Instance = null;

        public static CompoundManager Instance
        {
            get { return m_Instance; }
        }

        public CompoundDescription[] m_Compounds;

        void Awake()
        {
            m_Instance = this;
        }
    }

    [Serializable]
    public class CompoundDescription
    {
        public Transform m_Prefab;
        public Texture m_Icon;
    }
}
