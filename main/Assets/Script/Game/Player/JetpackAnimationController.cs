using UnityEngine;

namespace Assets.Script.Player
{
    public class JetpackAnimationController : MonoBehaviour
    {
        public delegate void EventAnimCallback();

        public event EventAnimCallback m_OnUndeployOver;

        public void OnUndeployOver()
        {
            m_OnUndeployOver();
        }
    }
}
