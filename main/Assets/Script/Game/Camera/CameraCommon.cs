using UnityEngine;

namespace Assets.Script.Game.Camera
{
    public class CameraCommon : MonoBehaviour
    {
        public GameObject m_Player;
        public float m_AltitudeSpeed;
        public float m_MinAltutide;
        public float m_MaxAltitude;
        public float m_MaxDistance;
        public float m_RotationSpeed;
        
        [HideInInspector]
        public float m_CameraAltitude = 0;

        [HideInInspector]
        public float m_YAngle = 0;

        public Vector3 DetectObstacle(Vector3 _position)
        {
            Vector3 origin = m_Player.transform.position;
            Vector3 direction = _position - origin;
            RaycastHit raycastInfo;

            int layerIdPlayer = LayerMask.NameToLayer("Player");
            int layerIdCameraRaycastIgnore = LayerMask.NameToLayer("CameraRaycastIgnore");
            int layerToIgnoreMask = (1 << layerIdPlayer) | (1 << layerIdCameraRaycastIgnore);
            int layerMask = ~layerToIgnoreMask;
            bool res = Physics.Raycast(origin, direction.normalized, out raycastInfo, direction.magnitude, layerMask);
            if (!res)
                return _position;

            float distanceFromObstacle = 3;
            return raycastInfo.point - direction.normalized * distanceFromObstacle;
        }
    }
}
