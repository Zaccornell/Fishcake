﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XboxCtrlrInput;
/*
 * Author: John Plant
 * Date: 2018/11/29
 */
public class CustomEventSystem : StandaloneInputModule
{
    public CustomInputModule m_cim;
	// Use this for initialization
	protected override void Awake ()
    {
		base.m_InputOverride = m_cim;
        base.Awake();
	}    
}
