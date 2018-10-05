using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * Author: John Plant
 * Date: 2018/10/5
 */

/*
 * Enemy behaviour for attacking the king
 * inherits from the base enemy state
 */

public class EnemyAttackKing : EnemyState
{
    public Actor m_king;
    public Actor[] m_players = null;
    public int m_damage = 10;

    private float m_attackSpeed;
    private float m_attackTimer;
    private float m_agroRange;

    public float AttackSpeed
    {
        get { return m_attackSpeed; }
        set { m_attackSpeed = value; }
    }
    public float AgroRange
    {
        get { return m_agroRange; }
        set { m_agroRange = value; }
    }

    /*
     * Constructor
     * Params:
     *      owner: reference to the state machine, to be passed into the base constructor
     *      players: array of all players in the game
     *      king: reference to the king
     *      attackSpeed: rate of attack in seconds
     *      agroRange: Distance before the enemy changes to the attack player state
     */
    public EnemyAttackKing(Enemy owner, Actor[] players, Actor king, float attackSpeed, float agroRange) : base(owner)
    {
        m_players = players;
        m_king = king;
        m_agroRange = agroRange;
    }

    /*
     * Handles assigning targets, attacking and state transitions
     */
    public override void Update()
    {
        m_attackTimer -= Time.deltaTime;
        m_target = m_king;

        foreach (Actor current in m_players)
        {
            if (!current.gameObject.activeSelf)
                continue;

            if ((current.transform.position - m_owner.transform.position).sqrMagnitude < m_agroRange * m_agroRange)
            {
                m_owner.ChangeState(1);
            }
        }

        if (m_attackTimer <= 0)
        {
            Collider[] targets = Physics.OverlapBox(m_owner.transform.position + m_owner.transform.forward, new Vector3(1, 1, 1));
            foreach(Collider current in targets)
            {
                if (current.gameObject == m_target.gameObject)
                {
                    m_target.TakeDamage(10, m_owner);
                    m_attackTimer = m_attackSpeed;
                }
            }
        }
    }

    /*
     * Function to get a new path between the enemy and the nearest position to the king
     */
    public override void UpdatePath(ref NavMeshPath path)
    {
        NavMeshHit hit = new NavMeshHit();
        NavMesh.SamplePosition(m_target.transform.position + (m_owner.transform.position - m_target.transform.position).normalized, out hit, 5, -1);
        NavMesh.CalculatePath(m_owner.transform.position, hit.position, -1, path);
    }
}
