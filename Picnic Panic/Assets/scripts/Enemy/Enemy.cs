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

    private int m_pathIndex = 0;
    private int m_updatePath = 0;
    private Actor m_attacker = null;
    private EnemyState[] m_states;
    private EnemyAttackPlayer m_attackPlayer;
    private EnemyAttackKing m_attackKing;
    private int m_stateIndex;


    // Behaviour tree
    //SelectorNode m_root;

    //    SelectorNode m_targetSide;

    //        SequenceNode m_attackingKing;

    //            IsSpecificActor m_targetIsKing;
    //            DistanceToActor m_kingAttackingRange;
    //            ToggleNode m_wasAttacked;
    //            SetTarget m_setTargetAttacker;

    //        SequenceNode m_updateTarget;

    //            S


    // Use this for initialization
    void Start()
    {
        m_health = m_maxHealth;
        m_rigidBody = GetComponent<Rigidbody>();
        m_path = new NavMeshPath();
        m_movement = new Vector3();
        m_stateIndex = 0;

        m_states = new EnemyState[2];
        m_attackKing = new EnemyAttackKing(this, m_players, m_king, m_attackDistance, m_attackRadius, m_attackSpeed, m_attackDamage, m_agroRange);
        m_states[0] = m_attackKing;

        m_attackPlayer = new EnemyAttackPlayer(this, m_players, m_attackDistance, m_attackRadius, m_attackSpeed, m_attackDamage, m_agroRange);
        m_states[1] = m_attackPlayer;        
    }

    public int PathIndex
    {
        get { return m_pathIndex; }
        set { m_pathIndex = value; }
    }
    public Vector3 Movement
    {
        get { return m_movement; }
        set { m_movement = value; }
    }

    private void FixedUpdate()
    {
        m_rigidBody.MovePosition(m_rigidBody.position + (m_movement * Time.deltaTime * m_speed));
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
            m_states[m_stateIndex].UpdatePath(ref m_path);
            m_updatePath = 5;
            m_pathIndex = 0;
            for(int i = 0; i < m_path.corners.Length; i++)
            {
                m_path.corners[i].y = m_height;
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

        //if (m_pathIndex == m_path.corners.Length - 1 && (transform.position - m_path.corners[m_pathIndex]).magnitude < 0.1)
        //{
        //    m_movement = Vector3.zero;
        //}
        
        m_movement.y = 0;
    }

    public override void TakeDamage(int damage, Actor attacker)    
    {
        m_health -= damage;
        // creating a knock back feel to the enemy once you hit it
        // using Velocity and distance to push the enemy back
        Vector3 dashVelocity = Vector3.Scale((gameObject.transform.position - attacker.gameObject.transform.position).normalized, m_knockBackDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
        m_rigidBody.AddForce(dashVelocity * m_rigidBody.mass, ForceMode.VelocityChange);
        if (m_health <= 0)
        {
            m_spawner.EnemyDeath(this);
            Destroy(gameObject);
        }
        m_attacker = attacker;
    }

    public void ChangeState(int index)
    {
        m_stateIndex = index;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.forward * m_attackDistance, m_attackRadius);
    }
}
