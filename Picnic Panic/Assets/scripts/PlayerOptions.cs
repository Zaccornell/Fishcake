using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: John Plant / Bradyn Corkill
 * Date: 2018/10/17
 * 
 * A singleton for storing the player options
 */
public class PlayerOptions
{
    private static readonly PlayerOptions m_instance = new PlayerOptions();

    public bool m_invulToggle;
    public bool m_vibrationToggle = true;
    public bool m_cutsceneToggle = true;
    public float m_SFX;
    public float m_music;
    public bool m_firendlyFire;
    public bool m_screenShake;


	public PlayerOptions()
    {

    }
	
	 
    static PlayerOptions()
    {

    }

    /*
     * Static function to get an instance of the function
     */
    public static PlayerOptions Instance
    {
        get { return m_instance; }
    }
}
