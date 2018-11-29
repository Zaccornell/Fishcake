using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*
 * Author: John Plant
 * Date: 2018/11/29
 * 
 * Basic ant controller for the main menu
 */
public class MainMenuAnt : MonoBehaviour
{
    public NavMeshAgent m_agent;

	// Use this for initialization
	void Start ()
    {

	}

    // Update is called once per frame
    void Update ()
    {
        // if the ant has reached its destination
		if ((m_agent.destination - transform.position).magnitude < 0.1)
        {
            Destroy(gameObject); // destroy the ant
        }
	}

    /*
     * Set the target of the NavMeshAgent
     * Params:
     *      pos: the position of the target
     */
    public void SetTarget(Vector3 pos)
    {
        // check the position is on the navmesh
        NavMeshHit hit = new NavMeshHit();
        NavMesh.SamplePosition(pos, out hit, 5, -1);

        transform.rotation = Quaternion.LookRotation((hit.position - transform.position).normalized); // start the ant looking at its destination
        m_agent.SetDestination(hit.position); // set the destination of the agent
    }
}
