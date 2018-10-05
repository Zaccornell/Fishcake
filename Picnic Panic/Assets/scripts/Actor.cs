using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public int m_maxHealth;

    protected int m_health;
    protected bool m_alive;

    public virtual void TakeDamage(int damage, Actor attacker)
    {

    }
}
