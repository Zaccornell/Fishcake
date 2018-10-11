using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * Author: John Plant
 * Date: 2018/10/4
 */

/*
Base class for the enemy behaviour states
 */
public abstract class EnemyState
{
    public Enemy m_owner;

    protected Actor m_target;

    /*
     * Constructor
     * Stores a reference to the enemy state machine
     */
    public EnemyState(Enemy owner)
    {
        m_owner = owner;
    }

    public Actor Target
    {
        get { return m_target; }
    }

    public abstract void Update();

    /*
     * Base attack function for the enemy
     * calls the take damage function on the target
     * Params: target actor and amount of damage
     */
    public virtual void Attack(Actor target, int damage)
    {
        target.TakeDamage(damage, m_owner);
    }

    /*
     * Abstact function declaration for getting a path
     * Takes in a reference to a NavMeshPath
     */
    public abstract void UpdatePath(ref NavMeshPath path);

}
