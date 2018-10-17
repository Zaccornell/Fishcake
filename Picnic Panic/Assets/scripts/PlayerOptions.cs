using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Aurther: John
 * Date: 2018/10/17
 * 
 * 
 */
public class PlayerOptions
{
    private static readonly PlayerOptions m_instance = new PlayerOptions();

    public bool m_invulToggle;

    static PlayerOptions()
    {

    }
    private PlayerOptions()
    {

    }

    public static PlayerOptions Instance
    {
        get { return m_instance; }
    }
}
