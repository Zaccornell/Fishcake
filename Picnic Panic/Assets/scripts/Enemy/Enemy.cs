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
    public float m_attackRange;
    public int m_attackDamage;
    public float m_agroRange;
    [HideInInspector] public Actor[] m_players = null;
    [HideInInspector] public Actor m_king = null;
    [HideInInspector] public Spawner m_spawner = null;
    [HideInInspector] public Actor m_target;
    [HideInInspector] public NavMeshPath m_path = null;

    private int m_pathIndex = 0;
    private int m_updatePath = 0;
    private Actor m_attacker = null;
    private bool m_wasAttacked;
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
        m_wasAttacked = false;
        m_stateIndex = 0;

        m_states = new EnemyState[2];
        m_attackKing = new EnemyAttackKing(this, m_players, m_king, m_attackSpeed, m_attackDamage, m_agroRange);
        m_states[0] = m_attackKing;

        m_attackPlayer = new EnemyAttackPlayer(this, m_players, m_attackRange, m_attackSpeed, m_attackDamage, m_agroRange);
        m_states[1] = m_attackPlayer;


        // Behaviour Tree
        //m_root = nzew SelectorNode();
        //{
        //    m_targetSide = new SelectorNode();
        //    m_root.AddChild(m_targetSide);
        //    {
        //        m_attackingKing = new SequenceNode();
        //        m_targetSide.AddChild(m_attackingKing);
        //        {
        //            m_targetIsKing = new IsSpecificActor();
        //            m_targetIsKing.m_target = m_king;
        //            m_attackingKing.AddChild(m_targetIsKing);

        //            m_kingAttackingRange = new DistanceToActor();
        //            m_kingAttackingRange.m_distance = m_attackRange;
        //            m_kingAttackingRange.m_target = m_king;
        //            m_attackingKing.AddChild(m_kingAttackingRange);

        //            m_wasAttacked = new ToggleNode();
        //            m_wasAttacked.m_toggled = false;
        //            m_attackingKing.AddChild(m_wasAttacked);

        //            m_setTargetAttacker = new SetTarget();
        //            m_attackingKing.AddChild(m_setTargetAttacker);
        //        }
        //    }
        //}
        
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
                m_path.corners[i].y = 0.5f;
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
        m_wasAttacked = false;
    }

    public override void TakeDamage(int damage, Actor attacker)    
    {
        m_health -= damage;
        // creating a knock back feel to the enemy once you hit it
        // using Velocity and distance to push the enemy back
        Vector3 dashVelocity = Vector3.Scale((gameObject.transform.position - attacker.gameObject.transform.position).normalized, m_knockBackDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
        m_rigidBody.AddForce(dashVelocity * m_rigidBody.mass);
        if (m_health <= 0)
        {
            m_spawner.GetComponent<Spawner>().EnemyDeath(gameObject);
            Destroy(gameObject);
        }
        m_attacker = attacker;
        m_wasAttacked = true;
    }

    public void ChangeState(int index)
    {
        m_stateIndex = index;
    }
}
