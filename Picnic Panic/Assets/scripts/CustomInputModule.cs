using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XboxCtrlrInput;

public class CustomInputModule : BaseInput
{
    public XboxController m_controller;

    public override float GetAxisRaw(string axisName)
    {
        if (axisName == "Horizontal")
        {
            return XCI.GetAxisRaw(XboxAxis.LeftStickX, m_controller);
        }
        else if (axisName == "Vertical")
        {
            return XCI.GetAxisRaw(XboxAxis.LeftStickY, m_controller);
        }
        else
        {
            return 0;
        }
    }

    public override bool GetButtonDown(string buttonName)
    {
        if (buttonName == "Submit")
        {
            return XCI.GetButtonDown(XboxButton.A, m_controller);
        }
        else if (buttonName == "Cancel")
        {
            return XCI.GetButtonDown(XboxButton.B, m_controller);
        }
        else
        {
            return false;
        }
    }
}
