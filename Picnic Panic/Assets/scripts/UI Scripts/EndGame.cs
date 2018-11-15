﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public enum Cause
{
    PieKing,
    Player
}

public class EndGame : MonoBehaviour
{
    public GameObject[] m_children;
    public GameObject[] m_objects;
    public Spawner m_spawner;
    public HUD m_hud;
    public Player[] m_players;
    public Text m_roundValue;
    public Text m_endCauseValue;
    public Text m_gameTimer;
    public GameObject[] m_playerStats;
    public Text[] m_player1Values;
    public Text[] m_player2Values;
    public Text[] m_player3Values;
    public Text[] m_player4Values;
    public float m_length;

    private float m_timer;
    private int m_minutes;

    private Cause m_cause;

    public Cause Cause
    {
        set { m_cause = value; }
    }

    // Use this for initialization
    void Start ()
    {
        m_timer = m_length;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_timer -= Time.deltaTime;
        if (m_timer <= 0)
        {
            foreach(GameObject current in m_children)
            {
                current.SetActive(true);
            }

            m_roundValue.text = "You survived for " + m_spawner.CurrentRound.ToString() + " rounds";

            if (m_cause == Cause.PieKing)
            {
                m_endCauseValue.text = "You let your king die";
            }
            else
            {
                m_endCauseValue.text = "You all died";
            }
            

            // checking to see if the timer is greater than 60 seconds 
            if (m_timer >= 60)
            {
                m_minutes =(int)m_timer / 60; // dividing it from the time 
                if (m_minutes > 1)
                {
                    m_gameTimer.text = "You survived for " + m_minutes.ToString() + "minutes" + m_hud.m_timer.ToString("0") + " seconds"; // checking to see if its more than a minute
                }
                else
                {
                    m_gameTimer.text = "You survived for " + m_minutes.ToString() + "minute" + m_hud.m_timer.ToString("0") + " seconds"; // only outputing when it is 1 minute 
                }
            }
            else
            {
                m_gameTimer.text = "You survived for " + m_hud.m_timer.ToString("0") + " seconds"; // if it doesnt go over a minute 

            }

            foreach (Player player in m_players)
            {
                switch (player.m_playerNumber)
                {
                    case 1:
                        m_playerStats[0].SetActive(true);
                        m_player1Values[0].text = player.Kills.ToString();
                        m_player1Values[1].text = player.Deaths.ToString();
                        m_player1Values[2].text = player.AntKills.ToString();
                        m_player1Values[3].text = player.RoachKills.ToString();
                        m_player1Values[4].text = player.HealsUsed.ToString();
                        break;

                    case 2:
                        m_playerStats[1].SetActive(true);
                        m_player2Values[0].text = player.Kills.ToString();
                        m_player2Values[1].text = player.Deaths.ToString();
                        m_player2Values[2].text = player.AntKills.ToString();
                        m_player2Values[3].text = player.RoachKills.ToString();
                        m_player2Values[4].text = player.HealsUsed.ToString();
                        break;

                    case 3:
                        m_playerStats[2].SetActive(true);
                        m_player3Values[0].text = player.Kills.ToString();
                        m_player3Values[1].text = player.Deaths.ToString();
                        m_player3Values[2].text = player.AntKills.ToString();
                        m_player3Values[3].text = player.RoachKills.ToString();
                        m_player3Values[4].text = player.HealsUsed.ToString();
                        break;

                    case 4:
                        m_playerStats[3].SetActive(true);
                        m_player4Values[0].text = player.Kills.ToString();
                        m_player4Values[1].text = player.Deaths.ToString();
                        m_player4Values[2].text = player.AntKills.ToString();
                        m_player4Values[3].text = player.RoachKills.ToString();
                        m_player4Values[4].text = player.HealsUsed.ToString();
                        break;
                }
            }

            Time.timeScale = 0;
        }
	}

    private void OnEnable()
    {
        foreach (Player player in m_spawner.m_players)
        {
            player.enabled = false;
            player.Animator.enabled = false;
        }
        foreach (MovingActor current in m_spawner.m_enemies)
        {
            current.enabled = false;
            ((Enemy)current).Animator.enabled = false;
        }
        foreach (GameObject child in m_objects) // going though and selecting every child inside of the array
        {
            child.SetActive(false); // turning the object off inside the array
        }
        m_spawner.enabled = false;             

        GamePad.SetVibration(PlayerIndex.One, 0, 0);
        GamePad.SetVibration(PlayerIndex.Two, 0, 0);
        GamePad.SetVibration(PlayerIndex.Three, 0, 0);
        GamePad.SetVibration(PlayerIndex.Four, 0, 0);        
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(1); // loading the scene up of the gmae
        Time.timeScale = 1;
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0); // loading the screen up for mainmenu
        Time.timeScale = 1;
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
