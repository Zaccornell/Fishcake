using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public GameObject[] m_lives;
    public Text[] displayHealth;
    public Actor[] actorHealth;
    public Text m_displayTimer;
    public Spawner m_timer;


    private int m_playerLives = 5;

    public int PlayerLives
    {
        get { return m_playerLives; }
        set { m_playerLives = value; }
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < actorHealth.Length; i++)
        {
            displayHealth[i].text = actorHealth[i].Health.ToString();
        }
        m_displayTimer.text = ((int)m_timer.RoundTimer).ToString();
    }

    public bool UseLife()
    {
        if (m_playerLives > 0)
        {
            m_lives[m_playerLives - 1].SetActive(false);
            m_playerLives--;
            return true;
        }
        else
        {
            return false;
        }
    }

}
