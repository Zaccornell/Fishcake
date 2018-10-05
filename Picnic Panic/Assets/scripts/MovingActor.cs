using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingActor : Actor
{
    public float m_speed;
    public float m_attackSpeed;
    public int m_attackDamage;

    protected float m_attackTimer;
    protected bool m_canAttack;
    protected Rigidbody m_rigidBody = null;
    protected Vector3 m_movement;
}
