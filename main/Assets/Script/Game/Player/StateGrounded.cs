using Assets.Script.Core.Gameplay;
using Assets.Script.Core.Math;
using Assets.Script.Core.MessageSystem;
using Assets.Script.Core.StateMachine;
using Assets.Script.Game.Compounds;
using Assets.Script.Game.Messages;

using System;
using UnityEngine;

#pragma warning disable 649
namespace Assets.Script.Player
{
    [Serializable]
    public class StateGrounded : State
    {
        [SerializeField]
        float m_WalkSpeed;

        [SerializeField]
        float m_RunSpeed;

        [SerializeField]
        float m_Height;

        [SerializeField]
        bool m_DrawLocalFrame;

        PlayerController m_Controller;

        FD_Cooldown m_DPadCooldown;

        enum InputMode
        {
            Default,
            BuildingSelection,
            BuildingConstruction
        }
        InputMode m_InputMode;

        GameObject m_BuildingInConstruction;
        Compound m_CompoundInConstruction;

        public override Enum GetId()
        {
            return PlayerStateId.Grounded;
        }

        public override void Init(MachineBehavior _machine)
        {
            m_Controller = _machine.gameObject.GetComponent<PlayerController>();
            m_DPadCooldown = new FD_Cooldown(0.2f);

            base.Init(_machine);
        }

        public override void OnEnter()
        {
            MessageManager.Instance.Connect(MessageId.MSG__GAME__BUILDING_SELECTED, BuildingToConstructSelected);

            MessageManager.Instance.Send(MessageId.MSG_UI_SHOWGROUNDEDCONTROL);
            MessageManager.Instance.Send(MessageId.MSG__UI__HIDE_BUILDING_PANEL);
            m_InputMode = InputMode.Default;
        }

        public override void OnFixedUpdate()
        {
            float hAxis = Input.GetAxis("Horizontal");
            float vAxis = Input.GetAxis("Vertical");

            if (hAxis != 0 || vAxis != 0)
            {
                float currentSpeed = m_WalkSpeed;
                if (Input.GetAxis("RT") > 0)
                    currentSpeed = m_RunSpeed;

                Vector3 cameraForward = Camera.main.transform.forward;
                Vector3 cameraRight = Camera.main.transform.right;
                float magnitudeForward = vAxis * currentSpeed * Time.fixedDeltaTime;
                float magnitudeRight = hAxis * currentSpeed * Time.fixedDeltaTime;

                Vector3 direction = cameraForward * magnitudeForward + cameraRight * magnitudeRight;
                m_Controller.Move(direction);

                SetUpOrientation();
                SetForwardOrientation();
            }

            RaycastHit raycast = m_Controller.GetGround();
            StickToGround(raycast);

            switch (m_InputMode)
            {
                case InputMode.Default:
                    HandleDefaultInput();
                    break;

                case InputMode.BuildingSelection:
                    HandleBuildingSelectionInput();
                    break;

                case InputMode.BuildingConstruction:
                    HandleBuildingConstruction();
                    HandleBuildingConstructionInput();
                    break;
            }

            if (m_DrawLocalFrame)
                m_Controller.DrawLocalFrame();
        }

        public override void OnExit()
        {
            MessageManager.Instance.Detach(MessageId.MSG__GAME__BUILDING_SELECTED, BuildingToConstructSelected);
        }

        void StickToGround(RaycastHit _raycastInfo)
        {
            Vector3 playerPositionInPlan = transform.position;
            playerPositionInPlan.z = 0;
            playerPositionInPlan.Normalize();
            Vector3 newPosition = _raycastInfo.point - playerPositionInPlan * m_Height * 0.5f;
            transform.position = newPosition;
        }

        void SetUpOrientation()
        {
            Vector3 idealDown = transform.position;
            idealDown.z = 0;
            idealDown.Normalize();

            //z axis
            float zAngle = FD_Math.GetSignedAngle(Vector3.right, idealDown, Vector3.forward);
            zAngle += 90;

            Vector3 eulerAngles = new Vector3(0, 0, zAngle);
            transform.eulerAngles = eulerAngles;
        }

        void SetForwardOrientation()
        {
            Vector3 right, up, forward;
            m_Controller.GetLocalFrame(out right, out up, out forward);
            Vector3 projectedCameraForward = FD_Math.GetProjectedVectorOnPlan(Camera.main.transform.forward, up);
            float lookAtAngle = FD_Math.GetSignedAngle(forward, projectedCameraForward.normalized, up);
            transform.Rotate(0, lookAtAngle, 0, Space.Self);
        }

        void HandleDefaultInput()
        {
            if (Input.GetButtonDown("Y"))
                m_Machine.ChangeState(PlayerStateId.Jetpack);
            else if (Input.GetButtonDown("X"))
            {
                MessageManager.Instance.Send(MessageId.MSG__UI__SHOW_BUILDING_PANEL);
                MessageManager.Instance.Send(MessageId.MSG__UI__SHOW_BUILDING_SELECTION_CONTROL);
                m_InputMode = InputMode.BuildingSelection;
            }
        }

        void HandleBuildingSelectionInput()
        {
            if (Input.GetAxis("DPadH") > 0 && m_DPadCooldown.IsElasped())
            {
                MessageManager.Instance.Send(MessageId.MSG__UI__SELECT_BUILDING_RIGHT);
                m_DPadCooldown.Reset();
            }
            else if (Input.GetAxis("DPadH") < 0 && m_DPadCooldown.IsElasped())
            {
                MessageManager.Instance.Send(MessageId.MSG__UI__SELECT_BUILDING_LEFT);
                m_DPadCooldown.Reset();
            }

            if(Input.GetButtonDown("B"))
            {
                MessageManager.Instance.Send(MessageId.MSG__UI__HIDE_BUILDING_PANEL);
                MessageManager.Instance.Send(MessageId.MSG_UI_SHOWGROUNDEDCONTROL);
                m_InputMode = InputMode.Default;
            }

            if(Input.GetButtonDown("A"))
            {
                MessageManager.Instance.Send(MessageId.MSG__UI__SELECT_CURRENT_BUILDING);
            }
        }

        void HandleBuildingConstructionInput()
        {
            if(Input.GetButtonDown("B"))
            {
                DestroyImmediate(m_BuildingInConstruction);
                m_InputMode = InputMode.Default;
                MessageManager.Instance.Send(MessageId.MSG_UI_SHOWGROUNDEDCONTROL);
            }

            if(Input.GetButtonDown("A"))
            {
                m_InputMode = InputMode.Default;
                MessageManager.Instance.Send(MessageId.MSG_UI_SHOWGROUNDEDCONTROL);
            }

            if(Input.GetButtonDown("LB"))
            {
                m_CompoundInConstruction.RotateCompound(90);
            }

            if(Input.GetButtonDown("RB"))
            {
                m_CompoundInConstruction.RotateCompound(-90);
            }
        }

        void HandleBuildingConstruction()
        {
            float distanceFromPlayer = 5;

            float maxX, maxY;
            m_CompoundInConstruction.ComputeExtend(out maxX, out maxY);
            float max = maxX > maxY ? maxX : maxY;

            Vector3 distance = Camera.main.transform.forward * (distanceFromPlayer + max);
            Vector3 position = transform.position + distance;
            //Assets.Script.Core.DebugDrawer.FD_DebugDrawer.DrawFrame(position, Vector3.right, Vector3.up, Vector3.forward, 20);
            m_CompoundInConstruction.BasePosition = position;
            m_CompoundInConstruction.SetCompoundTransform();
        }

        void BuildingToConstructSelected(IMessage _msg)
        {
            BuildingToConstruct message = _msg.GetMessage<BuildingToConstruct>();

            //create new instance of object
            Transform gameObjectTransform = (Transform)Instantiate(message.m_Prefab, Vector3.zero, Quaternion.identity);
            m_BuildingInConstruction = gameObjectTransform.gameObject;
            m_CompoundInConstruction = m_BuildingInConstruction.GetComponent<Compound>();

            //switch input mode to construction
            m_InputMode = InputMode.BuildingConstruction;
            MessageManager.Instance.Send(MessageId.MSG__UI__SHOW_BUILDING_CONSTRUCTION_CONTROL);
        }
    }
}
