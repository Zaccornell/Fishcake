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
    public float m_attackDistance;
    public float m_attackRadius;
    public float m_windUpLength = 0.5f;

    private float m_attackSpeed;
    private float m_attackTimer;
    private float m_windUpTimer;
    private bool m_playerAttackable;

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
    public EnemyAttackPlayer(Enemy owner, Actor[] players, float attackDistance, float attackRadius, float attackSpeed, int attackDamage, float agroRange, float windUpLength) : base(owner)
    {
        m_players = players;
        m_attackDistance = attackDistance;
        m_attackRadius = attackRadius;
        m_attackSpeed = attackSpeed;
        m_attackDamage = attackDamage;
        m_agroRange = agroRange + 5;
        m_windUpLength = windUpLength;
    }

    /*
     * Handles assigning targets, attacking and state transitions
     */
	public override void Update ()
    {
        m_attackTimer -= Time.deltaTime;
        m_windUpTimer -= Time.deltaTime;

        List<Actor> playersInRange = new List<Actor>();
        List<float> distanceToPlayers = new List<float>();
        foreach (Actor current in m_players)
        {
            if (!current.gameObject.activeSelf || !current.Alive)
                continue;

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

        // Check if player is in attack range
        if (m_attackTimer <= 0 && m_target != null)
        {
            Collider[] targets = Physics.OverlapSphere(m_owner.transform.position + m_owner.transform.forward * m_attackDistance, m_attackRadius);
            bool foundPlayer = false;
            foreach (Collider current in targets)
            {
                if ( current.gameObject == m_target.gameObject)
                {
                    if (m_playerAttackable == false)
                    {
                        m_playerAttackable = true;
                        m_windUpTimer = m_windUpLength;
                    }
                    foundPlayer = true;
                }
                                
            }
            if (foundPlayer == false)
            {
                m_playerAttackable = false;
            }
        }
        else
        {
            m_playerAttackable = false;
        }

        // if the player has been in attack range for the duration of the wind up
        // attack the player
        if (m_windUpTimer <= 0 && m_playerAttackable && m_attackTimer <= 0)
        {
            Collider[] targets = Physics.OverlapSphere(m_owner.transform.position + m_owner.transform.forward * m_attackDistance, m_attackRadius);
            foreach (Collider current in targets)
            {                 
                if (m_target.gameObject == current.gameObject)
                {
                    if (m_owner.m_enemyAttack.Length > 0)
                    {
                        int index = Random.Range(0, m_owner.m_enemyAttack.Length);
                        if (m_owner.m_enemyAttack[index] != null)
                        {
                            m_owner.m_audioSourceSFX.PlayOneShot(m_owner.m_enemyAttack[index]);
                        }
                    }                   
                }                                                      
            }
            m_owner.Animator.SetTrigger("Attack");
            m_attackTimer = m_attackSpeed;
            m_playerAttackable = false;
            m_windUpTimer = m_windUpLength;

            if (!m_target.gameObject.activeSelf || !m_target.Alive)
            {
                m_target = null;
            }
        }
    }

    /*
     * Function to get a new path between the enemy and the target
     */
    public override void UpdatePath(ref NavMeshPath path, int areaMask)
    {
        if (m_target != null)
        {
            NavMesh.CalculatePath(m_owner.transform.position, m_target.transform.position, areaMask, path);
        }
    }

    public override void Attack()
    {
        if (m_target)
        {
            m_target.TakeDamage(m_attackDamage, m_owner);
        }
    }
}
