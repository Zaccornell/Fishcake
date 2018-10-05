using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *Author: Bradyn Corkill
 * Date: 2018/10/3
 */
public class PieKing : Actor
{
    public int m_restoreHealth; // setting the amout of health to restore per round

	// Use this for initialization
	void Start ()
    {
        m_health = m_maxHealth; // setting the health to the max health 
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // calling from another source to damage the king
    public override void TakeDamage(int damage, Actor attacker)
    {
        m_health -= damage; // taking damage 
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

    // to end the game 
    void EndGame()
    {
        if (m_health <= 0) // if the kings health is less than or is 0 
        {
             // call end game
        }
    }
}
