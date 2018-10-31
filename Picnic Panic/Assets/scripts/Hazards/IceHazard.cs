using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnTriggerEnter(Collider other)
    {
        MovingActor actor = other.gameObject.GetComponent<MovingActor>();

        if (actor != null)
        {
            actor.m_useForce = true;

            m_affectedPlayers.Add(actor);
            m_originalDrag.Add(actor.RigidBody.drag);

            actor.RigidBody.drag = 0;

            actor.RigidBody.AddForce(actor.Movement * actor.m_speed, ForceMode.VelocityChange);            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MovingActor actor = other.gameObject.GetComponent<MovingActor>();

        if (actor != null)
        {
            actor.m_useForce = false;

            int index = m_affectedPlayers.IndexOf(actor);
            actor.RigidBody.drag = m_originalDrag[index];

            m_affectedPlayers.RemoveAt(index);
            m_originalDrag.RemoveAt(index);            
        }
    }
}
