﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*
 * Author: Bradyn Corkill / John Plant
 * Date: 2018/10/5
 * 
 * The main ant class and the statemachine for the ant
 */
public class Enemy : MovingActor
{
    public float m_windUpLength;
    public float m_knockBackDistance; // to able the designer to give a value how far the knock back will be 
    public float m_attackDistance;
    public float m_attackRadius;
    public float m_agroRange;
    public float m_height;
    public AudioSource m_audioSourceSFX;
    public AudioClip[] m_enemyDeath;
    public AudioClip[] m_enemyFall;
    public AudioClip[] m_enemyAttack;
    public ParticleSystem m_eatingKing;
    public GameObject m_deathParticles;

    private Actor[] m_players = null;
    private Actor m_king = null;
    private Spawner m_spawner = null;
    private NavMeshPath m_path = null;
    private int m_pathIndex = 0;
    private int m_updatePath = 0;
    private EnemyState[] m_states;
    private EnemyAttackPlayer m_attackPlayer;
    private EnemyAttackKing m_attackKing;
    private int m_stateIndex;
    private int m_areaMask;

    public Actor[] Players
    {
        set { m_players = value; }
    }
    public Actor King
    {
        set { m_king = value; }
    }
    public Spawner Spawner
    {
        set { m_spawner = value; }
    }
    // Use this for initialization
    void Start()
    {
        m_health = m_maxHealth;
        m_rigidBody = GetComponent<Rigidbody>();
        m_path = new NavMeshPath();
        m_movement = new Vector3();
        m_stateIndex = 0;
        m_alive = true;
        m_areaMask = (1 << 0) + (0 << 1) + (1 << 2) + (1 << 3);

        m_states = new EnemyState[2];
        m_attackKing = new EnemyAttackKing(this, m_players, m_king, m_attackDistance, m_attackRadius, m_attackSpeed, m_attackDamage, m_agroRange);
        m_states[0] = m_attackKing;

        m_attackPlayer = new EnemyAttackPlayer(this, m_players, m_attackDistance, m_attackRadius, m_attackSpeed, m_attackDamage, m_agroRange, m_windUpLength);
        m_states[1] = m_attackPlayer;

        m_animator = GetComponent<Animator>();
    }

    public int PathIndex
    {
        get { return m_pathIndex; }
        set { m_pathIndex = value; }
    }

    /*
     * Handles rigid body movement and rotation
     */
    private void FixedUpdate()
    {
        // if the rigidbody should use force to move the ant
        if (m_useForce)
        {
            m_rigidBody.AddForce(m_movement * m_speed * 2, ForceMode.Acceleration);
        }
        else
        {
            m_rigidBody.MovePosition(m_rigidBody.position + (m_movement * Time.deltaTime * m_speed));
        }

        // if the ant is moving look in that direction
        if (m_movement.magnitude != 0)
            m_rigidBody.rotation = Quaternion.LookRotation(m_movement.normalized);
    }
    // Update is called once per frame
    void Update()
    {
        m_states[m_stateIndex].Update(); // update the current state

        // When the path should be updated
        if (m_updatePath == 0)
        {
            m_states[m_stateIndex].UpdatePath(ref m_path, m_areaMask);  // get the new path from the current state
            
            // Reset path variables
            m_updatePath = 5;
            m_pathIndex = 0;
            for(int i = 0; i < m_path.corners.Length; i++)
            {
                m_path.corners[i].y = transform.position.y;
            }
        }
        m_updatePath--;

        // When the ant has reached the current path node
        if (m_pathIndex < m_path.corners.Length && (transform.position - m_path.corners[m_pathIndex]).magnitude < 0.1)
        {
            m_pathIndex++; // move to the next node
        }
        // if the current path index does not exceed the array
        if (m_pathIndex < m_path.corners.Length)
            m_movement = (m_path.corners[m_pathIndex] - transform.position).normalized; // get the direction of movement from the current position to the path node
        else
            m_movement = Vector3.zero;
        
        m_movement.y = 0;

        m_animator.SetFloat("Speed", m_movement.magnitude); // set the speed variable of the animator to the magnitude of movement
    }

    /*
     * Causes the ant to take damage and handles the death of the ant
     * Params:
     *      Damage: the amount of damage to take
     *      Attacker: the actor that attacked the ant
     */
    public override void TakeDamage(int damage, Actor attacker)    
    {
        // if the ant is currently alive
        if (m_alive)
        {
            m_health -= damage; // take away the damage
            // if the damage caused the cockroach to die
            if (m_health <= 0)
            {
                // check for sounds in the enemy death array and play a random one
                if (m_enemyDeath.Length > 0)
                {
                    int index = Random.Range(0, m_enemyDeath.Length);
                    if (m_enemyDeath[index] != null)
                    {
                        m_audioSourceSFX.PlayOneShot(m_enemyDeath[index]);
                    }
                }
                m_spawner.EnemyDeath(this);
                m_alive = false;

                // replace the ant with the death particles
                Instantiate(m_deathParticles, transform.position, transform.rotation);
                Destroy(gameObject);
            }

            // if the attacker exists and the ant is still alive
            if (attacker != null && m_alive)
            {
                // creating a knock back feel to the enemy once you hit it
                // using Velocity and distance to push the enemy back
                Vector3 dashVelocity = Vector3.Scale((gameObject.transform.position - attacker.gameObject.transform.position).normalized, m_knockBackDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
                m_rigidBody.AddForce(dashVelocity * m_rigidBody.mass, ForceMode.VelocityChange);
            }
        }
    }

    /*
     * Handles changing the current state
     * Param:
     *      Index: the index in the state array to change to
     */
    public void ChangeState(int index)
    {
        m_stateIndex = index;
        m_animator.SetTrigger("StateChanged"); // trigger the StateChanged variable to interupt the attack
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.forward * m_attackDistance, m_attackRadius);
    }

    public void FallDamage(int damage)
    {
        m_health -= damage;
        if (m_health <= 0)
        {
            if (m_enemyFall.Length > 0)
            {


                int index = Random.Range(0, m_enemyFall.Length);
                if (m_enemyFall[index] != null)
                {
                    m_audioSourceSFX.PlayOneShot(m_enemyFall[index]);

                }
            }
            m_spawner.EnemyDeath(this);
            m_alive = false;
            Destroy(gameObject);
        }
    }

    /*
     * The actual damage for the attack that is called during the attack animation
     * Calls the attack function on the current state
     */
    public void Attack()
    {
        // if the ant is alive
        if (m_alive)
        {
            m_states[m_stateIndex].Attack(); // call the attack function of the current state
        }
    }
}
