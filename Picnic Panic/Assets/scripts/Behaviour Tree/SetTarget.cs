using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTarget : BaseNode
{
    Actor m_target;
	// Use this for initialization
	void Start ()
    {
		
	}

    public override BehaviourResult Run(Actor actor)
    {
        if (m_target != null)
            ((Enemy)actor).m_target = m_target;
        return BehaviourResult.Success;
    }
}
