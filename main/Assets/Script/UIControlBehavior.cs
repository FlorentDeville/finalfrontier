using Assets.Script;
using Assets.Script.Core.MessageSystem;
using Assets.Script.Game.Compounds;
using Assets.Script.Game.Messages;

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
public class UIControlBehavior : MonoBehaviour 
{
    enum Button
    {
        Button_A,
        Button_B,
        Button_X,
        Button_Y,
        Button_LT,
        Button_RT,
        Button_LB,
        Button_RB
    }

    [SerializeField]
    RawImage[] m_Image;

    [SerializeField]
    Text[] m_Text;

    [Header("Buildings")]
    [SerializeField]
    GameObject m_BuildingPanel;

    [SerializeField]
    RawImage[] m_BuildingImages;

    [SerializeField]
    GameObject m_SelectionOverlay;

    int m_SelectedBuildingId;

	// Use this for initialization
	void OnEnable () 
    {
        MessageManager.Instance.Connect(MessageId.MSG_UI_SHOWGROUNDEDCONTROL, ShowGroundedControlUI);
        MessageManager.Instance.Connect(MessageId.MSG_UI_SHOWJETPACKCONTROL, ShowJetpackControlUI);
        MessageManager.Instance.Connect(MessageId.MSG_UI_SHOWFREEFALLCONTROL, ShowFreefallControlUI);
        MessageManager.Instance.Connect(MessageId.MSG__UI__SHOW_BUILDING_SELECTION_CONTROL, ShowBuildingSelectionControlUI);
        MessageManager.Instance.Connect(MessageId.MSG__UI__SHOW_BUILDING_CONSTRUCTION_CONTROL, ShowBuildingConstructionControlUI);

        MessageManager.Instance.Connect(MessageId.MSG__UI__SHOW_BUILDING_PANEL, ShowBuildingPanel);
        MessageManager.Instance.Connect(MessageId.MSG__UI__HIDE_BUILDING_PANEL, HideBuildingPanel);
        MessageManager.Instance.Connect(MessageId.MSG__UI__SELECT_BUILDING_LEFT, SelectLeftBuilding);
        MessageManager.Instance.Connect(MessageId.MSG__UI__SELECT_BUILDING_RIGHT, SelectRightBuilding);
        MessageManager.Instance.Connect(MessageId.MSG__UI__SELECT_CURRENT_BUILDING, SelectCurrentBuildingToConstruct);

        HideBuildingPanel(null);
	}

    void OnDisable()
    {
        MessageManager.Instance.Detach(MessageId.MSG_UI_SHOWGROUNDEDCONTROL, ShowGroundedControlUI);
        MessageManager.Instance.Detach(MessageId.MSG_UI_SHOWJETPACKCONTROL, ShowJetpackControlUI);
        MessageManager.Instance.Detach(MessageId.MSG_UI_SHOWFREEFALLCONTROL, ShowFreefallControlUI);
        MessageManager.Instance.Detach(MessageId.MSG__UI__SHOW_BUILDING_SELECTION_CONTROL, ShowBuildingSelectionControlUI);
        MessageManager.Instance.Detach(MessageId.MSG__UI__SHOW_BUILDING_CONSTRUCTION_CONTROL, ShowBuildingConstructionControlUI);

        MessageManager.Instance.Detach(MessageId.MSG__UI__SHOW_BUILDING_PANEL, ShowBuildingPanel);
        MessageManager.Instance.Detach(MessageId.MSG__UI__HIDE_BUILDING_PANEL, HideBuildingPanel);
        MessageManager.Instance.Detach(MessageId.MSG__UI__SELECT_BUILDING_LEFT, SelectLeftBuilding);
        MessageManager.Instance.Detach(MessageId.MSG__UI__SELECT_BUILDING_RIGHT, SelectRightBuilding);
        MessageManager.Instance.Detach(MessageId.MSG__UI__SELECT_CURRENT_BUILDING, SelectCurrentBuildingToConstruct);
    }

    void ShowGroundedControlUI(IMessage _msg)
    {
        HideButton(Button.Button_A);
        HideButton(Button.Button_B);
        ShowButton(Button.Button_X, "BUILDINGS");
        ShowButton(Button.Button_Y, "JETPACK");
        ShowButton(Button.Button_RT, "RUN");
        HideButton(Button.Button_LT);
        HideButton(Button.Button_LB);
        HideButton(Button.Button_RB);
    }

    void ShowBuildingSelectionControlUI(IMessage _msg)
    {
        ShowButton(Button.Button_A, "SELECT");
        ShowButton(Button.Button_B, "CLOSE");
        HideButton(Button.Button_X);
        HideButton(Button.Button_Y);
        ShowButton(Button.Button_RT, "RUN");
        HideButton(Button.Button_LT);
        HideButton(Button.Button_LB);
        HideButton(Button.Button_RB);
    }

    void ShowBuildingConstructionControlUI(IMessage _msg)
    {
        ShowButton(Button.Button_A, "BUILD");
        ShowButton(Button.Button_B, "CLOSE");
        HideButton(Button.Button_X);
        HideButton(Button.Button_Y);
        ShowButton(Button.Button_RT, "RUN");
        HideButton(Button.Button_LT);
        ShowButton(Button.Button_LB, "TURN");
        ShowButton(Button.Button_RB, "TURN");
    }

    void ShowJetpackControlUI(IMessage _msg)
    {
        HideButton(Button.Button_A);
        ShowButton(Button.Button_B, "DOWN");
        ShowButton(Button.Button_X, "UP");
        ShowButton(Button.Button_Y, "FREE FALL");
        HideButton(Button.Button_LT);
        ShowButton(Button.Button_RT, "SPEED UP");
        ShowButton(Button.Button_LB, "TILT LEFT");
        ShowButton(Button.Button_RB, "TILT RIGHT");
    }

    void ShowFreefallControlUI(IMessage _msg)
    {
        HideButton(Button.Button_A);
        HideButton(Button.Button_B);
        HideButton(Button.Button_X);
        ShowButton(Button.Button_Y, "JETPACK");
        HideButton(Button.Button_LT);
        HideButton(Button.Button_RT);
        HideButton(Button.Button_LB);
        HideButton(Button.Button_RB);
    }

    void ShowBuildingPanel(IMessage _msg)
    {
        m_BuildingPanel.SetActive(true);

        CompoundDescription[] descriptions = CompoundManager.Instance.m_Compounds;
        for (int i = 0; i < descriptions.Length; ++i)
        {
            m_BuildingImages[i].texture = descriptions[i].m_Icon;
        }


        MoveOverlayToBuilding(0);
        m_SelectedBuildingId = 0;
    }

    void HideBuildingPanel(IMessage _msg)
    {
        m_BuildingPanel.SetActive(false);
    }

    void SelectLeftBuilding(IMessage _msg)
    {
        if (m_SelectedBuildingId <= 0)
            return;

        --m_SelectedBuildingId;
        MoveOverlayToBuilding(m_SelectedBuildingId);
    }

    void SelectRightBuilding(IMessage _msg)
    {
        if (m_SelectedBuildingId >= m_BuildingImages.Length - 1)
            return;

        ++m_SelectedBuildingId;
        MoveOverlayToBuilding(m_SelectedBuildingId);
    }

    void ShowButton(Button _button, string _text)
    {
        int index = (int)_button;
        m_Text[index].text = _text;
        m_Text[index].gameObject.SetActive(true);
        m_Image[index].gameObject.SetActive(true);
    }

    void HideButton(Button _button)
    {
        int index = (int)_button;
        m_Text[index].gameObject.SetActive(false);
    }

    void MoveOverlayToBuilding(int _id)
    {
        if (_id < 0 || _id > m_BuildingImages.Length)
            return;

        m_SelectionOverlay.transform.position = m_BuildingImages[_id].transform.position;
    }

    void SelectCurrentBuildingToConstruct(IMessage _msg)
    {
        CompoundDescription description = CompoundManager.Instance.m_Compounds[m_SelectedBuildingId];
        BuildingToConstruct msg = new BuildingToConstruct(description.m_Prefab);
        MessageManager.Instance.Send(msg);

        MessageManager.Instance.Send(MessageId.MSG__UI__HIDE_BUILDING_PANEL);
    }
}
