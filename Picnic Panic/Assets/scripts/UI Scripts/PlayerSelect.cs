using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSelect : MonoBehaviour
{
    public Image[] m_players;
    private bool[] m_ready;
	// Use this for initialization
	void Start ()
    {
        m_ready = new bool[4];
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetButtonDown("Submit" + (i + 1).ToString()))
            {
                if (!m_players[i].enabled)
                {
                    m_players[i].enabled = true;
                }
                else
                {
                    m_ready[i] = true;
                }
            }
        }

        int selected = 0;
        for (int i = 0; i < 4; i++)
        {
            if (m_players[i].enabled)
            {
                selected++;
            }
        }
        int ready = 0;
        for (int i = 0; i < 4; i++)
        {
            if (m_ready[i])
            {
                ready++;
            }
        }

        if (selected != 0 && selected == ready)
        {
            SceneManager.LoadScene(0);
        }

    }
}
