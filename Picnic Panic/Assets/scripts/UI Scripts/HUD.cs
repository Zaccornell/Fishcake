using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public GameObject[] m_lives;
    public Text[] m_playerDisplays;
    public Player[] m_players;
    public Actor m_king;
    public Text m_kingDisplay;
    public Text m_displayTimer;
    public Spawner m_spanwer;

    public Text m_enemyCounter;

    private int m_playerLives = 5;

    public int PlayerLives
    {
        get { return m_playerLives; }
        set { m_playerLives = value; }
    }

	// Use this for initialization
	void Start ()
    {
		foreach (Text current in m_playerDisplays)
        {
            current.enabled = false;
        }

	}
	
	// Update is called once per frame
	void Update ()
    {
        foreach (Player current in m_players)
        {
            m_playerDisplays[current.m_playerNumber - 1].text = current.Health.ToString();
            m_playerDisplays[current.m_playerNumber - 1].enabled = true;
        }

        m_kingDisplay.text = m_king.Health.ToString();

        m_displayTimer.text = (Mathf.Ceil(m_spanwer.RoundTimer)).ToString(); // displays timer onto the HUD
        m_enemyCounter.text = (m_spanwer.EnemyTotal).ToString();// displays the amount of enemies left to spawn and spawned
       

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
