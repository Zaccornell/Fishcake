using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
{
	// Use this for initialization
	void Start ()
    {
		
	}
	
	public override BehaviourResult Run(Actor actor)
    {
        foreach(BaseNode current in m_children)
        {
            if (current.Run(actor) == BehaviourResult.Success)
                return BehaviourResult.Success;
        }
        return BehaviourResult.Failure;
    }
}
