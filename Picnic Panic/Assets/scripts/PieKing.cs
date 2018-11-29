using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 *Author: Bradyn Corkill / John Plant
 * Date: 2018/10/3
 */
public class PieKing : Actor
{
    public Spawner m_spawner; // calling spanwer class
    public int m_restoreHealth; // setting the amout of health to restore per round
    public ScreenShake m_shake;
    public Image m_warningBorder;

    private Slider m_healthBar;
    //public Mesh[] m_pieKingMeshStates;
    //public Material[] m_pieKingMaterialStates;

    //private MeshFilter m_filter;
    //private MeshRenderer m_renderer;

	// Use this for initialization
	void Start ()
    {
        m_health = m_maxHealth; // setting the health to the max health 
        m_alive = true;
        m_healthBar = GetComponentInChildren<Slider>();
        //m_filter = GetComponent<MeshFilter>();
        //m_renderer = GetComponent<MeshRenderer>();
	}

    void Awake()
    {
        m_spawner.OnRoundEnd += new MyDel(RoundEnd); // Adding the RoundEnd function to the spawner's OnRoundEnd event
    }

    // Update is called once per frame
    void Update ()
    {
        
	}

    // calling from another source to damage the king
    public override void TakeDamage(int damage, Actor attacker)
    {
        // if god mode is not turned on
		if (!PlayerOptions.Instance.m_invulToggle)
		{
        	m_health -= damage; // taking damage 
            m_healthBar.value = m_health; // update the health bar

            // start the screen shake
            if (m_shake != null)
			    m_shake.StartShake();

            // if the king was killed
            if (m_health < 0)
            {
                m_health = 0;                
            }

            // if the king's health is less than or equal to 30%
            if ((float)m_health / m_maxHealth <= 0.3)
            {
                m_warningBorder.enabled = true; // enable the warning border
            }
		}

        //float percent = m_health / (float)m_maxHealth;

        //if (percent < 0.3)
        //{
        //    m_filter.mesh = m_pieKingMeshStates[2];
        //    m_renderer.material = m_pieKingMaterialStates[2];
        //}
        //else if (percent < 0.6)
        //{
        //    m_filter.mesh = m_pieKingMeshStates[1];
        //    m_renderer.material = m_pieKingMaterialStates[1];
        //}
        //else if (percent < 0.9)
        //{
        //    m_filter.mesh = m_pieKingMeshStates[0];
        //    m_renderer.material = m_pieKingMaterialStates[0];
        //}
    }

    // end of each round will call this function
    void RoundEnd()
    {
        m_health += m_restoreHealth; // restoring the health
        m_healthBar.value = m_health; // update the health bar
        if (m_health >= m_maxHealth) // if the health is higher than max health
        {
            m_health = m_maxHealth; // reset health to max health 
        }
        if ((float)m_health / m_maxHealth > 0.3)
        {
            m_warningBorder.enabled = false;
        }
    }

    /*
     * Resets the kings health back to full
     */
    public void ResetValues()
    {
        m_health = m_maxHealth;
    }
}
