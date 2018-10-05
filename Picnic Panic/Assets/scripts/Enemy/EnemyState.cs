using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState
{
    public Enemy m_owner;

    protected float m_attackRange;
    protected Actor m_target;

    public EnemyState(Enemy owner)
    {
        m_owner = owner;
    }

    public Actor Target
    {
        get { return m_target; }
    }
    public float AttackRange
    {
        get { return m_attackRange; }
        set { m_attackRange = value; }
    }
	
	public virtual void Update ()
    {
		
	}

    public virtual void Attack(Actor target, int damage)
    {
        target.TakeDamage(damage, m_owner);
    }
}
