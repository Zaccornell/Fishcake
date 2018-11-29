using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author:John Plant
 * Date: 2018/11/29
 */
public class DestroyParticleEffect : MonoBehaviour
{
    ParticleSystem[] m_particleSystem;

	// Use this for initialization
	void Start ()
    {
        m_particleSystem = GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // check if all the particle systems are finished
        bool destroy = true;
        foreach (ParticleSystem current in m_particleSystem)
        {
            if (current.IsAlive())
            {
                destroy = false;
                break;
            }
        }

        // if they are destroy the gameobject
		if (destroy)
        {
            Destroy(gameObject);
        }
	}
}
