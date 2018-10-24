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
    public int m_attackDamage = 10;
    public float m_attackDistance;
    public float m_attackRadius;

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
    public EnemyAttackKing(Enemy owner, Actor[] players, Actor king, float attackDistance, float attackRadius, float attackSpeed, int attackDamage, float agroRange) : base(owner)
    {
        m_players = players;
        m_king = king;
        m_attackSpeed = attackSpeed;
        m_attackDistance = attackDistance;
        m_attackRadius = attackRadius;
        m_attackDamage = attackDamage;
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
            if (!current.gameObject.activeSelf || !current.Alive)
                continue;

            if ((current.transform.position - m_owner.transform.position).sqrMagnitude < m_agroRange * m_agroRange)
            {
                m_owner.ChangeState(1);
            }
        }

        if (m_attackTimer <= 0)
        {
            Collider[] targets = Physics.OverlapSphere(m_owner.transform.position + m_owner.transform.forward * m_attackDistance, m_attackRadius);
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
    public override void UpdatePath(ref NavMeshPath path, int areaMask)
    {
        NavMeshHit hit = new NavMeshHit();
        NavMesh.SamplePosition(m_target.transform.position + (m_owner.transform.position - m_target.transform.position).normalized, out hit, 5, areaMask);
        NavMesh.CalculatePath(m_owner.transform.position, hit.position, areaMask, path);
    }
}
