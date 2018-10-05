using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceToActor : BaseNode
{
    public Actor m_target;
    public float m_distance;
	// Use this for initialization
	void Start ()
    {
		
	}

    public override BehaviourResult Run(Actor actor)
    {
        if ((actor.transform.position - m_target.transform.position).magnitude <= m_distance)
        {
            return BehaviourResult.Success;
        }
        else
        {
            return BehaviourResult.Failure;
        }
    }
}
