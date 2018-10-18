using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public virtual void TakeDamage(int damage, Actor attacker)
    {

    }
}
