using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : BaseNode
{

	// Use this for initialization
	void Start ()
    {
		
	}

    public override BehaviourResult Run(Actor actor)
    {
        Enemy enemy = (Enemy)actor;
        if (enemy.PathIndex < enemy.m_path.corners.Length)
            enemy.Movement = (enemy.m_path.corners[enemy.PathIndex] - enemy.transform.position).normalized;
        else
            enemy.Movement = Vector3.zero;
        return BehaviourResult.Success;
    }
}
