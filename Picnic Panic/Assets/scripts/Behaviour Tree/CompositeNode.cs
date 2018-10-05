using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : BaseNode
{
    protected List<BaseNode> m_children;
	// Use this for initialization
	void Start ()
    {
		
	}

    public void AddChild(BaseNode child)
    {
        m_children.Add(child);
    }
}
