using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navmeshtest : MonoBehaviour
{
    public GameObject m_target;
    private NavMeshPath m_path;
	// Use this for initialization
	void Start ()
    {
        m_path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, m_target.transform.position, NavMesh.AllAreas, m_path);
        for (int i = 0; i < m_path.corners.Length - 1; i++)
        {
            Debug.DrawLine(m_path.corners[i], m_path.corners[i + 1], Color.red, 10);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
