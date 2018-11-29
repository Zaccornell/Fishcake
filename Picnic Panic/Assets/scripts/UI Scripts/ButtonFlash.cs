using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Author: John Plant
 * Date: 2018/11/29
 */
public enum Direction
{
    Up,
    Right,
    Down,
    Left
}


public class ButtonFlash : MonoBehaviour
{
    public Image[] m_arrows;
    public Sprite m_baseSprite;
    public Sprite m_highlightSprite;
    public float m_flashDuration;

    private float[] m_flashTimer;

	// Use this for initialization
	void Start ()
    {
        m_flashTimer = new float[4];
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < 4; i++)
        {
            // Count down each of the timers
            if (m_flashTimer[i] > 0)
                m_flashTimer[i] -= Time.deltaTime;

            // if the current timer is has finished
            if (m_flashTimer[i] <= 0)
            {
                m_arrows[i].sprite = m_baseSprite; // change the sprite of the of the array back to the base sprite
            }
        }        
	}

    /*
     * Start the flash timer for the direction specified
     * Params:
     *      direction: the direction of the arrow to flash
     */
    public void Flash(Direction direction)
    {
        m_flashTimer[(int)direction] = m_flashDuration; // start the flash timer of the specified direction
        m_arrows[(int)direction].sprite = m_highlightSprite; // change the sprite of the arrow to the highlighted sprite
    }

    /*
     * Enable the arrow images
     */
    public void EnableArrows()
    {
        foreach (Image current in m_arrows)
        {
            current.enabled = true;
        }
    }
}
