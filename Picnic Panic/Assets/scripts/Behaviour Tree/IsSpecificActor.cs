using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsSpecificActor : BaseNode
{
    public Actor m_testAgainst;
    public Actor m_target;
	// Use this for initialization
	void Start ()
    {
		
	}

    public override BehaviourResult Run(Actor actor)
    {
        if (m_testAgainst == m_target)
        {
            return BehaviourResult.Success;
        }
        else
        {
            return BehaviourResult.Failure;
        }
    }
}
