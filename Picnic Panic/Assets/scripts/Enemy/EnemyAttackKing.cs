using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackKing : EnemyState
{
    public Actor m_king;
    public int m_damage = 10;

    private float m_attackSpeed;
    private float m_attackTimer;

    public float AttackSpeed
    {
        get { return m_attackSpeed; }
        set { m_attackSpeed = value; }
    }

    public EnemyAttackKing(Enemy owner, Actor king, float attackRange) : base(owner)
    {
        m_king = king;
        m_attackRange = attackRange;
    }

    public override void Update()
    {
        m_attackTimer -= Time.deltaTime;

        m_target = m_king;
    }
}
