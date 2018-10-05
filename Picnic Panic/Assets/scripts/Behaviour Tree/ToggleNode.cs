using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleNode : BaseNode
{
    public bool m_toggled;
	// Use this for initialization
	void Start ()
    {
		
	}

    public override BehaviourResult Run(Actor actor)
    {
        if (m_toggled)
        {
            return BehaviourResult.Success;
        }
        else
        {
            return BehaviourResult.Failure;
        }
    }
}
