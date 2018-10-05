using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetPath : BaseNode
{
    GameObject m_target;
	// Use this for initialization
	void Start ()
    {
		
	}

    public override BehaviourResult Run(Actor actor)
    {
        if (NavMesh.CalculatePath(actor.transform.position, m_target.transform.position, -1, ((Enemy)actor).m_path))
        {
            return BehaviourResult.Success;
        }
        else
        {
            return BehaviourResult.Failure;
        }
    }
}
