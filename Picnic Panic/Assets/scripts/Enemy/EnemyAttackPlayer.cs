using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackPlayer : EnemyState
{
    public Actor[] m_players = null;
    public int m_damage = 10;

    private float m_attackSpeed;
    private float m_attackTimer;

    public float AttackSpeed
    {
        get { return m_attackSpeed; }
        set { m_attackSpeed = value; }
    }

    public EnemyAttackPlayer(Enemy owner, Actor[] players, float attackRange) : base(owner)
    {
        m_players = players;
        m_attackRange = attackRange;
    }

	public override void Update ()
    {
        m_attackTimer -= Time.deltaTime;

        bool playerInRange = false;
        foreach (Actor current in m_players)
        {
            if (!current.gameObject.activeSelf)
                continue;

            if (m_target == null || ((current.transform.position - m_owner.transform.position).sqrMagnitude < (m_target.transform.position - m_owner.transform.position).sqrMagnitude))
            {
                m_target = current;
                playerInRange = true;
                break;
            }
        }

        if (!playerInRange)
        {

        }

        if (m_attackTimer <= 0)
        {
            if ((m_target.transform.position - m_owner.transform.position).sqrMagnitude <= m_attackRange * m_attackRange)
            {
                Attack(m_target, m_damage);
                m_attackTimer = m_attackSpeed;

                if (!m_target.gameObject.activeSelf)
                {
                    m_target = null;
                }
            }
        }
    }
}
