using Assets.Script.Core.MessageSystem;
using UnityEngine;

namespace Assets.Script.Game.Messages
{
    public class BuildingToConstruct : IMessage
    {
        public Transform m_Prefab;

        public BuildingToConstruct(Transform _prefab)
            : base(MessageId.MSG__GAME__BUILDING_SELECTED)
        {
            m_Prefab = _prefab;
        }
    }
}
