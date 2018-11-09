using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class StopVibrationOnQuit : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
        GamePad.SetVibration(PlayerIndex.Two, 0, 0);
        GamePad.SetVibration(PlayerIndex.Three, 0, 0);
        GamePad.SetVibration(PlayerIndex.Four, 0, 0);
    }
}
