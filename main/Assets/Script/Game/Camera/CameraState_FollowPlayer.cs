using Assets.Script.Core.Math;
using Assets.Script.Core.MessageSystem;
using Assets.Script.Core.StateMachine;
using Assets.Script.Player;
using UnityEngine;

namespace Assets.Script.Game.Camera
{
    public class CameraState_FollowPlayer : State
    {
        private CameraCommon m_Common;

        public override System.Enum GetId()
        {
            return CameraStateId.FollowPlayer;
        }

        public override void Init(MachineBehavior _machine)
        {
            base.Init(_machine);
            m_Common = gameObject.GetComponent<CameraCommon>();
        }

        public override void OnLateUpdate()
        {
            Vector3 playerUp = m_Common.m_Player.transform.localToWorldMatrix.GetColumn(1);

            transform.position = new Vector3(0, 0, 0);
            transform.eulerAngles = new Vector3(0, 0, 0);

            m_Common.m_CameraAltitude += Input.GetAxis("RightVertical") * m_Common.m_AltitudeSpeed * Time.fixedDeltaTime;
            m_Common.m_CameraAltitude = Mathf.Clamp(m_Common.m_CameraAltitude, m_Common.m_MinAltutide, m_Common.m_MaxAltitude);

            float YRotation = Input.GetAxis("RightHorizontal");
            m_Common.m_YAngle += YRotation * m_Common.m_RotationSpeed * Time.fixedDeltaTime;

            float zAngle = FD_Math.GetSignedAngle(Vector3.right, -playerUp, Vector3.forward);
            zAngle += 90;

            transform.Translate(m_Common.m_Player.transform.position, Space.Self); //move to player's position
            transform.Rotate(0, 0, zAngle, Space.Self); //align with Y axis of player
            transform.Rotate(playerUp, m_Common.m_YAngle, Space.World); //rotate around the y axis
            transform.Translate(0, m_Common.m_CameraAltitude, m_Common.m_MaxDistance, Space.Self); //offset of the camera position
            transform.LookAt(m_Common.m_Player.transform, playerUp); //look at the player

            transform.position = m_Common.DetectObstacle(transform.position); //check for obstacle
        }
    }
}
