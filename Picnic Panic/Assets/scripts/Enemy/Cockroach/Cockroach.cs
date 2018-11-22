using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * The cockroach controller
 */
public class Cockroach : MovingActor
{
    public float m_knockBackDistance;
    public float m_attackDistance;
    public float m_attackRadius;
    public float m_agroRange;
    public float m_height;
    public AudioSource m_audioSourceSFX;
    public AudioClip[] m_enemyDeath;
    public AudioClip[] m_enemyFall;
    public AudioClip[] m_enemyAttack;
    public ParticleSystem m_eatingKing;

    private int m_pathIndex = 0;
    private int m_updatePath = 0;
    private int m_areaMask;
    private Actor m_king = null;
    private Spawner m_spawner = null;
    private NavMeshPath m_path = null;

    public Actor King
    {
        get { return m_king; }
        set { m_king = value; }
    }
    public Spawner Spawner
    {
        get { return m_spawner; }
        set { m_spawner = Spawner; }
    }

    // Use this for initialization
    void Start ()
    {
        m_health = m_maxHealth;
        m_rigidBody = GetComponent<Rigidbody>();
        m_path = new NavMeshPath();
        m_movement = new Vector3();
        m_alive = true;
        m_animator = GetComponent<Animator>();

        m_areaMask = (1 << 0) + (0 << 1) + (1 << 2) + (0 << 3);
    }

    /*
     * Handles moving the rigidbody and the rotation
     */
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

        if (m_movement.magnitude != 0)
            m_rigidBody.rotation = Quaternion.LookRotation(m_movement.normalized);
    }

    // Update is called once per frame
    void Update ()
    {
        m_attackTimer -= Time.deltaTime;

        bool kingInRange = false;
        // check if the king is in the attack bubble
        Collider[] targetstemp = Physics.OverlapSphere(transform.position + transform.forward * m_attackDistance, m_attackRadius);
        foreach (Collider current in targetstemp)
        {
            if (current.gameObject == m_king.gameObject)
            {
                kingInRange = true;
            }
        }

        // if the cockroach can attack
        if (m_attackTimer <= 0)
        {
            // if the king is in range
            if (kingInRange)
            {
                // play the attack animation
                m_animator.SetTrigger("Attack");
                m_attackTimer = m_attackSpeed;
            }            
        }

        // if the cockroach should update it's path
        if (m_updatePath == 0)
        {
            NavMeshHit hit = new NavMeshHit();
            NavMesh.SamplePosition(m_king.transform.position + (transform.position - m_king.transform.position).normalized, out hit, 5, m_areaMask);
            NavMesh.CalculatePath(transform.position, hit.position, m_areaMask, m_path);

            m_updatePath = 5;
            m_pathIndex = 0;
            for (int i = 0; i < m_path.corners.Length; i++)
            {
                m_path.corners[i].y = transform.position.y;
            }
        }

        m_updatePath--;

        // if the cockroach should move onto the next path node
        if (m_pathIndex < m_path.corners.Length && (transform.position - m_path.corners[m_pathIndex]).magnitude < 0.1)
        {
            m_pathIndex++;
        }
        if (m_pathIndex < m_path.corners.Length && !kingInRange)
            m_movement = (m_path.corners[m_pathIndex] - transform.position).normalized;
        else
            m_movement = Vector3.zero;

        m_movement.y = 0;        

        m_animator.SetFloat("Speed", kingInRange ? 0 : m_movement.magnitude);
    }

    /*
     * Handles causing the cockroach to take damage from attacks
     * Params: 
     *      damage: the amount of damage
     *      Attacker: the actor the attacked the cockroach
     */
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
                        m_audioSourceSFX.PlayOneShot(m_enemyDeath[index]); // play sound at random in array 
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
                m_rigidBody.AddForce(dashVelocity, ForceMode.VelocityChange);
            }
        }
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
     * The actual damage that is called during the attack animation
     */
    public void Attack()
    {
        m_king.TakeDamage(m_attackDamage, this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.forward * m_attackDistance, m_attackRadius);
    }
}
