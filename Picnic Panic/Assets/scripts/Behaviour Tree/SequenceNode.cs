using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : CompositeNode
{
	// Use this for initialization
	void Start ()
    {
		
	}
	
	public override BehaviourResult Run(Actor actor)
    {
        foreach(BaseNode current in m_children)
        {
            if (current.Run(actor) == BehaviourResult.Failure)
                return BehaviourResult.Failure;
        }
        return BehaviourResult.Success;
    }
}
