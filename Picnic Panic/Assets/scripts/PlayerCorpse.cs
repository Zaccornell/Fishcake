using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCorpse : MonoBehaviour
{
    private float m_timer = 5;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_timer -= Time.deltaTime;

        if (m_timer <= 0)
        {
            Destroy(gameObject);
        }
	}
}
