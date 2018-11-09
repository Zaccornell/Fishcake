using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *Author: Bradyn Corkill
 * Date: 2018/10/3
 */
public class PieKing : Actor
{
    public Spawner m_spawner; // calling spanwer class
    public int m_restoreHealth; // setting the amout of health to restore per round
    public ScreenShake m_shake;
    //public Mesh[] m_pieKingMeshStates;
    //public Material[] m_pieKingMaterialStates;

    //private MeshFilter m_filter;
    //private MeshRenderer m_renderer;

	// Use this for initialization
	void Start ()
    {
        m_health = m_maxHealth; // setting the health to the max health 
        m_alive = true;

        //m_filter = GetComponent<MeshFilter>();
        //m_renderer = GetComponent<MeshRenderer>();
	}

    void Awake()
    {
        m_spawner.OnRoundEnd += new MyDel(RoundEnd); // it heals at the end of rounds 
    }

    // Update is called once per frame
    void Update ()
    {
    
	}

    // calling from another source to damage the king
    public override void TakeDamage(int damage, Actor attacker)
    {
		if (!PlayerOptions.Instance.m_invulToggle)
		{
        	m_health -= damage; // taking damage 
            if (m_shake != null)
			    m_shake.StartShake();
		}
        if (m_health < 0)
        {
            m_health -= damage; // taking damage 
            if (m_health < 0)
            {
                m_health = 0;
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
        if (m_health >= m_maxHealth) // if the health is higher than max health
        {
            m_health = m_maxHealth; // reset health to max health 
        }
    }

    public void ResetValues()
    {
        m_health = m_maxHealth;
    }
}
