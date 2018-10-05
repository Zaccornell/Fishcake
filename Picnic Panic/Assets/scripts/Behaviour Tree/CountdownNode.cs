using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownNode : BaseNode
{
    public int m_count;
    private int m_currentCount;
	// Use this for initialization
	void Start ()
    {
        m_currentCount = m_count;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_currentCount--;
	}

    public override BehaviourResult Run(Actor actor)
    {
        if (m_currentCount <= 0)
        {
            m_currentCount = m_count;
            return BehaviourResult.Success;
        }
        else
        {
            return BehaviourResult.Failure;
        }
    }
}
