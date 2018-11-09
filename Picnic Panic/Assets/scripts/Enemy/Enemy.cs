using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*
 * Author: Bradyn Corkill / John Plant
 * Date: 2018/10/5
 */
public class Enemy : MovingActor
{
    public float m_knockBackDistance; // to able the designer to give a value how far the knock back will be 
    public float m_attackDistance;
    public float m_attackRadius;
    public float m_agroRange;
    public float m_height;
    [HideInInspector] public Actor[] m_players = null;
    [HideInInspector] public Actor m_king = null;
    [HideInInspector] public Spawner m_spawner = null;
    [HideInInspector] public Actor m_target;
    [HideInInspector] public NavMeshPath m_path = null;
    public AudioSource m_audioSourceSFX;
    public AudioClip[] m_enemyDeath;
    public AudioClip[] m_enemyFall;
    public AudioClip[] m_enemyAttack;
    public ParticleSystem m_eatingKing;

    private int m_pathIndex = 0;
    private int m_updatePath = 0;
    private EnemyState[] m_states;
    private EnemyAttackPlayer m_attackPlayer;
    private EnemyAttackKing m_attackKing;
    private int m_stateIndex;
    private int m_areaMask;
    private Animator m_animator;

    public Animator Animator
    {
        get { return m_animator; }
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

        m_attackPlayer = new EnemyAttackPlayer(this, m_players, m_attackDistance, m_attackRadius, m_attackSpeed, m_attackDamage, m_agroRange);
        m_states[1] = m_attackPlayer;

        m_animator = GetComponent<Animator>();
    }

    public int PathIndex
    {
        get { return m_pathIndex; }
        set { m_pathIndex = value; }
    }

    private void FixedUpdate()
    {
        if (m_useForce)
        {
            m_rigidBody.AddForce(m_movement * m_speed * 2, ForceMode.Acceleration);
        }
        else
        {
            m_rigidBody.MovePosition(m_rigidBody.position + (m_movement * Time.deltaTime * m_speed));
        }

        if (m_rigidBody.velocity.magnitude > m_speed)
        {
            m_rigidBody.velocity = m_rigidBody.velocity.normalized * m_speed;
        }

        if (m_movement.magnitude != 0)
            m_rigidBody.rotation = Quaternion.LookRotation(m_movement.normalized);
    }
    // Update is called once per frame
    void Update()
    {
        m_states[m_stateIndex].Update();
        m_target = m_states[m_stateIndex].Target;

        if (m_updatePath == 0)
        {
            //NavMesh.CalculatePath(transform.position, m_target.transform.position, -1, m_path);
            m_states[m_stateIndex].UpdatePath(ref m_path, m_areaMask);
            m_updatePath = 5;
            m_pathIndex = 0;
            for(int i = 0; i < m_path.corners.Length; i++)
            {
                m_path.corners[i].y = transform.position.y;
            }
        }
        m_updatePath--;

        if (m_pathIndex < m_path.corners.Length && (transform.position - m_path.corners[m_pathIndex]).magnitude < 0.1)
        {
            m_pathIndex++;
        }
        if (m_pathIndex < m_path.corners.Length)
            m_movement = (m_path.corners[m_pathIndex] - transform.position).normalized;
        else
            m_movement = Vector3.zero;
        
        m_movement.y = 0;

        m_animator.SetFloat("Speed", m_movement.magnitude);
    }

    public override void TakeDamage(int damage, Actor attacker)    
    {
        if (m_alive)
        {
            m_health -= damage;
          
            if (m_health <= 0)
            {
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
                Destroy(gameObject);
            }

            if (attacker != null && m_alive)
            {
                // creating a knock back feel to the enemy once you hit it
                // using Velocity and distance to push the enemy back
                Vector3 dashVelocity = Vector3.Scale((gameObject.transform.position - attacker.gameObject.transform.position).normalized, m_knockBackDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
                m_rigidBody.AddForce(dashVelocity * m_rigidBody.mass, ForceMode.VelocityChange);
            }
        }
    }

    public void ChangeState(int index)
    {
        m_stateIndex = index;
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

    public void Attack()
    {
        m_states[m_stateIndex].Attack();
    }
}
