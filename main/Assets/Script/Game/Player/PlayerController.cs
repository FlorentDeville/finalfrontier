using Assets.Script.Core.Math;
using UnityEngine;

#pragma warning disable 649
namespace Assets.Script.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        float m_ColliderRadius;

        public float ColliderRadius
        {
            get { return m_ColliderRadius; }
        }

        /// <summary>
        /// Move the player using _displacement. It collides with all the layers but the layer Player.
        /// </summary>
        /// <param name="_displacement"></param>
        public void Move(Vector3 _displacement)
        {
            Ray collisionRay = new Ray(transform.position, _displacement.normalized);
            RaycastHit hit;
            int layerId = LayerMask.NameToLayer("Player");
            int layerMask = 1 << layerId;
            layerMask = ~layerMask;
            if (Physics.Raycast(collisionRay, out hit, _displacement.magnitude + m_ColliderRadius, layerMask))
                transform.position = hit.point - _displacement.normalized * m_ColliderRadius;
            else
                transform.position += _displacement;
        }

        /// <summary>
        /// Get a raycast to the closest object in the Ground layer. The ray goes from the player to the outer rim.
        /// </summary>
        /// <returns></returns>
        public RaycastHit GetGround()
        {
            Vector3 origin = transform.position;
            origin.x = 0;
            origin.y = 0;
            Vector3 downPlayer = (transform.position - origin).normalized;
            RaycastHit raycastInfo;
            int layer = 1 << LayerMask.NameToLayer("Ground");
            bool res = Physics.Raycast(origin, downPlayer, out raycastInfo, float.MaxValue, layer);
            if (!res)
                Debug.LogError("Player raycast to stick to ground failed.");

            return raycastInfo;
        }

        /// <summary>
        /// Return a local frame for the player as if he was up on the ground. It's actual orientation is not used.
        /// </summary>
        /// <param name="_right"></param>
        /// <param name="_up"></param>
        /// <param name="_forward"></param>
        public void GetLocalFrame(out Vector3 _right, out Vector3 _up, out Vector3 _forward)
        {
            _up = GetPlayerUp();
            _forward = transform.forward;
            _right = Vector3.Cross(_up, _forward);
        }

        /// <summary>
        /// Return the Up vector of the player when on the ground
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPlayerUp()
        {
            Vector3 up = -transform.position;
            up.z = 0;
            up.Normalize();
            return up;
        }

        public void DrawLocalFrame()
        {
            const float length = 5;

            Vector3 right, up, forward;
            GetLocalFrame(out right, out up, out forward);
            Debug.DrawLine(transform.position, transform.position + right * length, Color.red);
            Debug.DrawLine(transform.position, transform.position + up * length, Color.green);
            Debug.DrawLine(transform.position, transform.position + forward * length, Color.blue);
        }

        //Return the angle around the z axis making the player up on the ground.
        public float GetPlayerUpOrientation()
        {
            Vector3 idealDown = transform.position;
            idealDown.z = 0;
            idealDown.Normalize();

            //z axis
            float zAngle = FD_Math.GetSignedAngle(Vector3.right, idealDown, Vector3.forward);
            zAngle += 90;

            return zAngle;
        }
    }
}
