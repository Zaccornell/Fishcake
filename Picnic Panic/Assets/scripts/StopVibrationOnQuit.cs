﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
/*
 * Author: John Plant
 * Date: 2018/11/29
 */
public class StopVibrationOnQuit : MonoBehaviour
{
    /*
     * Simply turns off vibration when the application is exitted
     */
    private void OnApplicationQuit()
    {
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
        GamePad.SetVibration(PlayerIndex.Two, 0, 0);
        GamePad.SetVibration(PlayerIndex.Three, 0, 0);
        GamePad.SetVibration(PlayerIndex.Four, 0, 0);
    }
}
