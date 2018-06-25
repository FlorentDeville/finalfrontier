using Assets.Script.Core.DebugDrawer;
using Assets.Script.Core.Math;
using Assets.Script.Core.MessageSystem;
using Assets.Script.Core.StateMachine;
using System;
using UnityEngine;

#pragma warning disable 649
namespace Assets.Script.Player
{
    public class StateJetpack : State
    {
        [SerializeField]
        float m_Speed;

        [SerializeField]
        float m_FastSpeed;

        [SerializeField]
        float m_RotationSpeed;

        [SerializeField]
        GameObject m_Jetpack;

        [SerializeField]
        string m_JetpackDeployTriggerName;

        [SerializeField]
        string m_JetpackUndeployTriggerName;

        PlayerController m_Controller;

        //float zAngle;
        Vector3 m_Position;
        Vector3 m_Orientation;

        public override Enum GetId()
        {
            return PlayerStateId.Jetpack;
        }

        public override void Init(MachineBehavior _machine)
        {
            m_Controller = _machine.gameObject.GetComponent<PlayerController>();
            DeactivateJetpack();
            base.Init(_machine);
            m_Jetpack.GetComponent<JetpackAnimationController>().m_OnUndeployOver += DeactivateJetpack;
        }

        public override void OnEnter()
        {
            MessageManager.Instance.Send(MessageId.MSG_UI_SHOWJETPACKCONTROL);
            ActivateJetpack();
            StartDeployAnimation();
            m_Position = transform.position;
            m_Orientation = transform.eulerAngles;
        }

        public override void OnFixedUpdate()
        {
            transform.position = new Vector3(0, 0, 0);
            transform.eulerAngles = new Vector3(0, 0, 0);

            transform.Translate(m_Position, Space.World);
            transform.Rotate(m_Orientation, Space.World);

            float currentSpeed = m_Speed;
            if (Input.GetAxis("RT") > 0)
                currentSpeed = m_FastSpeed;

            Vector3 up = transform.localToWorldMatrix.GetColumn(1);
            Vector3 forward = transform.localToWorldMatrix.GetColumn(2);

            {   //rotation around forward axis
                float zAngle = 0;
                if (Input.GetButton("LB"))
                    zAngle += m_RotationSpeed * Time.fixedDeltaTime;
                else if (Input.GetButton("RB"))
                    zAngle -= m_RotationSpeed * Time.fixedDeltaTime;

                transform.Rotate(0, 0, zAngle, Space.World);
            }

            {   //translation
                Vector3 offset = Vector3.zero;
                if (Input.GetButton("X"))
                    offset += up * currentSpeed * Time.fixedDeltaTime;
                if (Input.GetButton("B"))
                    offset -= up * currentSpeed * Time.fixedDeltaTime;

                Vector3 cameraForward = Camera.main.transform.localToWorldMatrix.GetColumn(2);
                Vector3 cameraRight = Camera.main.transform.localToWorldMatrix.GetColumn(0);
                float hAxis = Input.GetAxis("Horizontal");
                float vAxis = Input.GetAxis("Vertical");
                offset += cameraForward * vAxis * currentSpeed * Time.fixedDeltaTime;
                offset += cameraRight * hAxis * currentSpeed * Time.fixedDeltaTime;

                m_Controller.Move(offset);

                if(hAxis != 0 || vAxis != 0)
                {
                    //rotation around up axis
                    Vector3 projectedCameraForward = FD_Math.GetProjectedVectorOnPlan(Camera.main.transform.forward, up);
                    float lookAtAngle = FD_Math.GetSignedAngle(forward, projectedCameraForward.normalized, up);
                    transform.Rotate(0, lookAtAngle, 0, Space.Self);
                }
            }
            
            //save position
            m_Position = transform.position;
            m_Orientation = transform.eulerAngles;

            if (Input.GetButtonDown("Y"))
                m_Machine.ChangeState(PlayerStateId.Freefall);
        }

        public override void OnExit()
        {
            Animator jetpackAnimator = m_Jetpack.GetComponent<Animator>();
            jetpackAnimator.SetTrigger(m_JetpackUndeployTriggerName);
        }

        private void DeactivateJetpack()
        {
            for (int i = 0; i < m_Jetpack.transform.childCount; ++i)
                m_Jetpack.transform.GetChild(i).gameObject.SetActive(false);
        }

        private void ActivateJetpack()
        {
            for (int i = 0; i < m_Jetpack.transform.childCount; ++i)
                m_Jetpack.transform.GetChild(i).gameObject.SetActive(true);
        }

        private void StartDeployAnimation()
        {
            Animator jetpackAnimator = m_Jetpack.GetComponent<Animator>();
            jetpackAnimator.SetTrigger(m_JetpackDeployTriggerName);

        }
    }
}
