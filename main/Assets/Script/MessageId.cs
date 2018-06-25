using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script
{
    public enum MessageId
    {
        //You have to create new message after MSG_NOMESSAGE but before MSG_COUNT
        MSG_NOMESSAGE,

        MSG_UI_SHOWGROUNDEDCONTROL,
        MSG_UI_SHOWJETPACKCONTROL,
        MSG_UI_SHOWFREEFALLCONTROL,
        MSG__UI__SHOW_BUILDING_SELECTION_CONTROL,
        MSG__UI__SHOW_BUILDING_CONSTRUCTION_CONTROL,

        MSG__UI__SHOW_BUILDING_PANEL,
        MSG__UI__HIDE_BUILDING_PANEL,
        MSG__UI__SELECT_BUILDING_LEFT,
        MSG__UI__SELECT_BUILDING_RIGHT,
        MSG__UI__SELECT_CURRENT_BUILDING,

        MSG__GAME__BUILDING_SELECTED,

        MSG_COUNT,
    }
}
