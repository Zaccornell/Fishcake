using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Image[] m_children;
    public Spawner m_spawner;
    public MonoBehaviour[] m_gameplayScripts;

    private bool m_active = false;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (Image child in m_children)
            {
                child.enabled = !m_active;
            }
            foreach (Enemy current in m_spawner.m_enemies)
            {
                current.enabled = m_active;
            }
            foreach (MonoBehaviour script in m_gameplayScripts)
            {
                script.enabled = m_active;
            }
            m_active = !m_active;
        }
	}
}
