using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public float m_speed;
    public int m_maxHealth;
    public float m_attackSpeed;

    protected int m_health;
    protected float m_attackTimer;
    protected bool m_canAttack;
    protected Rigidbody m_rigidBody = null;
    protected Vector3 m_movement;
    protected bool m_alive;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public virtual void TakeDamage(int damage, Actor attacker)
    {

    }
}
