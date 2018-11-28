using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Author: John Plant
 * Date: 2018/11/29
 */
public class Actor : MonoBehaviour
{
    public int m_maxHealth;

    protected int m_health;
    protected bool m_alive;

    public int Health
    {
        get { return m_health; }
        set { m_health = value; }
    }
    
    public bool Alive
    {
        get { return m_alive; }
        set { m_alive = value; }
    }

    /*
     * The base take damage function for the actor class
     * Params:
     *      Damage: The amount of damage for the actor to take
     *      Attacker: The actor the attacked the current one
     */
    public virtual void TakeDamage(int damage, Actor attacker)
    {

    }
}
