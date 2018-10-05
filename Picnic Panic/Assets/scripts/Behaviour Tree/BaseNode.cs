using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviourResult
{
    Success,
    Failure
}

public abstract class BaseNode : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
		
	}

    public abstract BehaviourResult Run(Actor actor);
}
