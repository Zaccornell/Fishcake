using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedHazard : MonoBehaviour
{
    public float m_speedChange;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        MovingActor actor = other.gameObject.GetComponent<MovingActor>();

        if (actor != null)
        {
            actor.m_speed += m_speedChange;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MovingActor actor = other.gameObject.GetComponent<MovingActor>();

        if (actor != null)
        {
            actor.m_speed -= m_speedChange;
        }
    }
}
