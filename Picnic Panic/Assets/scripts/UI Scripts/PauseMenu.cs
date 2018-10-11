using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject[] m_children;
    public Spawner m_spawner;
    public Player[] m_players;
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
            foreach (GameObject child in m_children)
            {
                child.SetActive(!m_active);
            }
            foreach (Enemy current in m_spawner.m_enemies)
            {
                current.enabled = m_active;
            }
            foreach (MonoBehaviour script in m_gameplayScripts)
            {
                script.enabled = m_active;
            }
            foreach (Player current in m_players)
            {
                current.enabled = m_active;
            }
            m_active = !m_active;
        }
	}

    public void ResetClick()
    {
        foreach (Player current in m_players)
        {
            current.ResetValues();
            current.Respawn();
        }
        m_spawner.ResetValues();
    }

    public void QuitClick()
    {
        SceneManager.LoadScene(0);
    }
}
