﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class PauseMenu : MonoBehaviour
{
    public Button m_resumeButton;
    public Button m_backButton;
    public GameObject[] m_optionMenu;
    public GameObject[] m_children;
    public Spawner m_spawner;
    public Player[] m_players;
    public PieKing m_king;
    public HUD m_hud;
    public MonoBehaviour[] m_gameplayScripts;

    private bool m_optionOpen;

    private bool m_active = false;
	// Use this for initialization
	void Start ()
    {
        Cursor.visible = false; // sets the cursor to be invisible
        Cursor.lockState = CursorLockMode.Locked; // set the cursor to be lcoked int he middle
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_optionOpen)
        {
		    if (Input.GetKeyDown(KeyCode.Escape) || XCI.GetButtonDown(XboxButton.Start))
            {
                 ToggleObjects();
            }

        }
        if (m_active) // if Puase Menu is up will ...
        {
            Cursor.lockState = CursorLockMode.None; // unlock cursor
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // keeped locked
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
        m_king.ResetValues();
        foreach (Player current in m_players)
        {
            XInputDotNetPure.GamePad.SetVibration((XInputDotNetPure.PlayerIndex)current.m_playerNumber - 1, 0, 0); //. set the vibration stregnth 

        }
    }

    public void QuitClick()
    {
        SceneManager.LoadScene(0);
    }

    public void ResumeClick()
    {
        ToggleObjects();
    }

    // 
    private void ToggleObjects()
    {
        foreach (Player current in m_players)
        {
            XInputDotNetPure.GamePad.SetVibration((XInputDotNetPure.PlayerIndex)current.m_playerNumber - 1, 0, 0); //. set the vibration stregnth 
        }

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

        m_resumeButton.Select();
        m_king.enabled = m_active;
        m_spawner.enabled = m_active;
        m_hud.gameObject.SetActive(m_active);
        Physics.autoSimulation = m_active;

        Cursor.visible = !m_active; // makes cursor visable
        m_active = !m_active;
    }


    public void OptionButton()
    {
        foreach (GameObject child in m_children)
        {
            child.SetActive(!m_active);
        }
        foreach (GameObject opchild in m_optionMenu)
        {
            opchild.SetActive(m_active);
        }

        m_backButton.Select();
        m_optionOpen = true;

    }
    public void BackButton()
    {
        foreach (GameObject child in m_children)
        {
            child.SetActive(m_active);
        }
        foreach (GameObject opchild in m_optionMenu)
        {
            opchild.SetActive(!m_active);
        }
        m_resumeButton.Select();
        m_optionOpen = false;
    }
}
