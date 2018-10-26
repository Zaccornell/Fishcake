using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cockroach : MovingActor
{
    public float m_knockBackDistance;
    public float m_attackDistance;
    public float m_attackRadius;
    public float m_agroRange;
    public float m_height;
    [HideInInspector] public Actor m_king = null;
    [HideInInspector] public Spawner m_spawner = null;
    [HideInInspector] public NavMeshPath m_path = null;

    private int m_pathIndex = 0;
    private int m_updatePath = 0;
    private int m_areaMask;

    // Use this for initialization
    void Start ()
    {
        m_health = m_maxHealth;
        m_rigidBody = GetComponent<Rigidbody>();
        m_path = new NavMeshPath();
        m_movement = new Vector3();
        m_alive = true;

        m_areaMask = (1 << 0) + (0 << 1) + (1 << 2) + (0 << 3);
    }

    private void FixedUpdate()
    {
        m_rigidBody.MovePosition(m_rigidBody.position + (m_movement * Time.deltaTime * m_speed));
        if (m_movement.magnitude != 0)
            m_rigidBody.rotation = Quaternion.LookRotation(m_movement.normalized);
    }

    // Update is called once per frame
    void Update ()
    {
        m_attackTimer -= Time.deltaTime;

        if (m_attackTimer <= 0)
        {
            Collider[] targets = Physics.OverlapSphere(transform.position + transform.forward * m_attackDistance, m_attackRadius);
            foreach (Collider current in targets)
            {
                if (current.gameObject == m_king.gameObject)
                {
                    m_king.TakeDamage(10, this);
                    m_attackTimer = m_attackSpeed;
                }
            }
        }

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

        if (m_pathIndex < m_path.corners.Length && (transform.position - m_path.corners[m_pathIndex]).magnitude < 0.1)
        {
            m_pathIndex++;
        }
        if (m_pathIndex < m_path.corners.Length)
            m_movement = (m_path.corners[m_pathIndex] - transform.position).normalized;
        else
            m_movement = Vector3.zero;

        m_movement.y = 0;
    }

    public override void TakeDamage(int damage, Actor attacker)
    {
        if (m_alive)
        {
            m_health -= damage;
            if (m_health <= 0)
            {
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

    public void FallDamage(int damage)
    {
        m_health -= damage;
        if (m_health <= 0)
        {
            m_spawner.EnemyDeath(this);
            m_alive = false;
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.forward * m_attackDistance, m_attackRadius);
    }
}
