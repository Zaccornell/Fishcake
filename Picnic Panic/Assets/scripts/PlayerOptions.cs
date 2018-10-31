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
    public bool m_vibrationToggle;
    public bool m_cutsceneToggle;
    public float m_SFX;
    public float m_music;
    public bool m_firendlyFire;


	public PlayerOptions()
    {
	    m_vibrationToggle = true;
    }
	
	 
    static PlayerOptions()
    {

    }

    public static PlayerOptions Instance
    {
        get { return m_instance; }
    }
}
