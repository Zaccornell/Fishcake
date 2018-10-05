using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerNode : BaseNode
{
    public float m_duration;
    private float m_timer;
	// Use this for initialization
	void Start ()
    {
        m_timer = m_duration;
	}

    void Update()
    {
        m_timer += Time.deltaTime;
    }

    public override BehaviourResult Run(Actor actor)
    {
        if (m_timer <= 0)
        {
            m_timer = m_duration;
            return BehaviourResult.Success;
        }
        else
        {
            return BehaviourResult.Failure;
        }
    }
}
