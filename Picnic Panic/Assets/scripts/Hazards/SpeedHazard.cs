using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: John Plant
 * Date: 2018/11/29
 */
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

    /*
     * Handles changing the speed of actors that enter the trigger
     */
    private void OnTriggerEnter(Collider other)
    {
        MovingActor actor = other.gameObject.GetComponent<MovingActor>();

        if (actor != null)
        {
            actor.m_speed += m_speedChange;
        }
    }

    /*
     * Handles reverting the speed of actors who leave the trigger
     */
    private void OnTriggerExit(Collider other)
    {
        MovingActor actor = other.gameObject.GetComponent<MovingActor>();

        if (actor != null)
        {
            actor.m_speed -= m_speedChange;
        }
    }
}
