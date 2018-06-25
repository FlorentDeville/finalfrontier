using Assets.Script.Core.MessageSystem;
using Assets.Script.Core.StateMachine;
using UnityEngine;

#pragma warning disable 649
namespace Assets.Script.Player
{
    public class StateFreefall : State
    {
        [SerializeField]
        float m_FreeFallGravity;

        [SerializeField]
        float m_TangentSpeed;

        float m_FreeFall_Speed;
        PlayerController m_Controller;

        public override System.Enum GetId()
        {
            return PlayerStateId.Freefall;
        }

        public override void Init(MachineBehavior _machine)
        {
            m_Controller = _machine.gameObject.GetComponent<PlayerController>();
            base.Init(_machine);
        }

        public override void OnEnter()
        {
            m_FreeFall_Speed = 0;
            MessageManager.Instance.Send(MessageId.MSG_UI_SHOWFREEFALLCONTROL);
        }

        public override void OnFixedUpdate()
        {
            Vector3 playerDown = transform.position;
            playerDown.z = 0;
            playerDown.Normalize();
            m_FreeFall_Speed += m_FreeFallGravity * Time.fixedDeltaTime;

            Vector3 movement = playerDown * m_FreeFall_Speed;

            Vector3 cameraForward = Camera.main.transform.localToWorldMatrix.GetColumn(1);
            Vector3 cameraRight = Camera.main.transform.localToWorldMatrix.GetColumn(0);
            float hAxis = Input.GetAxis("Horizontal");
            float vAxis = Input.GetAxis("Vertical");

            if (vAxis != 0)
            {
                Vector3 directionForward = cameraForward - Vector3.Dot(cameraForward, playerDown) * playerDown;
                directionForward.Normalize();
                movement += directionForward * vAxis * m_TangentSpeed * Time.fixedDeltaTime;
            }
            if (hAxis != 0)
            {
                Vector3 directionRight = cameraRight - Vector3.Dot(cameraRight, playerDown) * playerDown;
                directionRight.Normalize();
                movement += directionRight * hAxis * m_TangentSpeed * Time.fixedDeltaTime;
            }

            Ray collisionRay = new Ray(transform.position, movement.normalized);
            RaycastHit hit;
            int layerId = LayerMask.NameToLayer("Player");
            int layerMask = 1 << layerId;
            layerMask = ~layerMask;
            if (Physics.Raycast(collisionRay, out hit, movement.magnitude + m_Controller.ColliderRadius, layerMask))
            {
                transform.position = hit.point - movement.normalized * m_Controller.ColliderRadius;
                m_Machine.ChangeState(PlayerStateId.Grounded);  
            }
            else
                transform.position += movement;

            if (Input.GetButtonDown("Y"))
                m_Machine.ChangeState(PlayerStateId.Jetpack);
        }
    }
}
