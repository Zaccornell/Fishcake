using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: John Plant
 * Date: 2018/11/29
 */
public class IceHazard : MonoBehaviour
{
    private List<MovingActor> m_affectedPlayers;
    private List<float> m_originalDrag;
    // Use this for initialization
    void Start()
    {
        m_affectedPlayers = new List<MovingActor>();
        m_originalDrag = new List<float>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    
    /*
     * Handles turning on force mode for actors the enter the trigger
     */
    private void OnTriggerEnter(Collider other)
    {
        // get the actor component of the object that entered
        MovingActor actor = other.gameObject.GetComponent<MovingActor>();

        // if the object that entered has an actor component
        if (actor != null)
        {
            actor.m_useForce = true; // Set the force mode of the actor to true

            m_affectedPlayers.Add(actor); // save actor
            m_originalDrag.Add(actor.RigidBody.drag); // save the actor's current drag

            actor.RigidBody.drag = 0; // remove drag from the actor's rigid body

            actor.RigidBody.AddForce(actor.Movement * actor.m_speed, ForceMode.VelocityChange); // set the velocity of the rigid body to its current movement
        }
    }

    /*
     * Handles turning off force mode when affected actors leave the trigger
     */
    private void OnTriggerExit(Collider other)
    {
        // get the actor component of the object that left
        MovingActor actor = other.gameObject.GetComponent<MovingActor>();

        // if the object has an actor component
        if (actor != null)
        {
            actor.m_useForce = false; // turn off force mode

            int index = m_affectedPlayers.IndexOf(actor); // get the index of the actor
            actor.RigidBody.drag = m_originalDrag[index]; // set the actor's drag back to what was saved when it entered

            // remove the player from the arrays
            m_affectedPlayers.RemoveAt(index);
            m_originalDrag.RemoveAt(index);            
        }
    }
}
