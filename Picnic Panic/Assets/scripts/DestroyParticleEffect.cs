using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        bool destroy = true;
        foreach (ParticleSystem current in m_particleSystem)
        {
            if (current.IsAlive())
            {
                destroy = false;
                break;
            }
        }

		if (destroy)
        {
            Destroy(gameObject);
        }
	}
}
