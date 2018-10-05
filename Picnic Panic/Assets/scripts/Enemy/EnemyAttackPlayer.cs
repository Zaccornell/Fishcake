using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * Author: John Plant
 * Date: 2018/10/4
 */

/*
 * Enemy behaviour for attacking the player
 * inherits from the base enemy state
 */
public class EnemyAttackPlayer : EnemyState
{
    public Actor[] m_players = null;
    public int m_attackDamage = 10;
    public float m_agroRange;

    private float m_attackSpeed;
    private float m_attackTimer;

    public float AttackSpeed
    {
        get { return m_attackSpeed; }
        set { m_attackSpeed = value; }
    }

    /*
     * Constructor
     * Params:
     *      owner: reference to the state machine, to be passed into the base constructor
     *      players: array of all players in the game
     */
    public EnemyAttackPlayer(Enemy owner, Actor[] players, float attackRange, float attackSpeed, int attackDamage, float agroRange) : base(owner)
    {
        m_players = players;
        m_attackRange = attackRange;
        m_agroRange = agroRange + 5;
        m_attackDamage = attackDamage;
    }

    /*
     * Handles assigning targets, attacking and state transitions
     */
	public override void Update ()
    {
        m_attackTimer -= Time.deltaTime;

        List<Actor> playersInRange = new List<Actor>();
        List<float> distanceToPlayers = new List<float>();
        foreach (Actor current in m_players)
        {
            if (!current.gameObject.activeSelf)
                continue;

            //if (m_target == null || ((current.transform.position - m_owner.transform.position).sqrMagnitude < (m_target.transform.position - m_owner.transform.position).sqrMagnitude))
            //{
            //    m_target = current;
            //    m_owner.m_target = m_target;
            //    break;
            //}
            //if (m_target != null)
            //{
            //    playerInRange = true;
            //}
            float distance = (current.transform.position - m_owner.transform.position).sqrMagnitude;
            if (distance <= m_agroRange * m_agroRange)
            {
                playersInRange.Add(current);
                distanceToPlayers.Add(distance);
            }
        }
        if (playersInRange.Count > 1)
        {
            int lowestIndex = 0;
            for(int i = 1; i < playersInRange.Count; i++)
            {
                if (distanceToPlayers[i] < distanceToPlayers[lowestIndex])
                {
                    lowestIndex = i;
                }
            }
            m_target = playersInRange[lowestIndex];
        }
        else if (playersInRange.Count == 1)
        {
            m_target = playersInRange[0];
        }
        else if (playersInRange.Count == 0)
        {
            m_owner.ChangeState(0);
        }

        if (m_attackTimer <= 0 && m_target != null)
        {
            if ((m_target.transform.position - m_owner.transform.position).sqrMagnitude <= m_attackRange * m_attackRange)
            {
                Attack(m_target, m_attackDamage);
                m_attackTimer = m_attackSpeed;

                if (!m_target.gameObject.activeSelf)
                {
                    m_target = null;
                }
            }
        }
    }

    /*
     * Function to get a new path between the enemy and the target
     */
    public override void UpdatePath(ref NavMeshPath path)
    {
        if (m_target != null)
            NavMesh.CalculatePath(m_owner.transform.position, m_target.transform.position, -1, path);
    }
}
