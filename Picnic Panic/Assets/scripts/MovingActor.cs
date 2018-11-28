using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: John Plant
 * Date: 2018/11/29
 */
public enum Type
{
    Player,
    Ant,
    Cockroach
}
public class MovingActor : Actor
{
    public float m_speed;
    public float m_attackSpeed;
    public int m_attackDamage;
    public bool m_useForce;
    public Type m_type;
    protected Animator m_animator;

    protected float m_attackTimer;
    protected bool m_canAttack;
    protected Rigidbody m_rigidBody = null;
    protected Vector3 m_movement;

    public Rigidbody RigidBody
    {
        get { return m_rigidBody; }
    }
    public Vector3 Movement
    {
        get { return m_movement; }
    }
    public Animator Animator
    {
        get { return m_animator; }
    }
}
