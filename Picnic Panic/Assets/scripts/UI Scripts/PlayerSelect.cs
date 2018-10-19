﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
public class PlayerSelect : MonoBehaviour
{
    public Text[] m_playerSlots;
    public Text m_startGame;
    public GameObject[] m_playerPrefabs;
    public Transform[] m_spawnPoints;
    public Spawner m_spawner;
    public HUD m_hud;
    public PauseMenu m_pauseMenu;
    public CameraControl m_camera;
    public MonoBehaviour[] m_gameplayScripts;

    private XboxController[] m_controllers;
    private KeyCode[] m_testButtons;
    private bool[] m_ready;
    private List<Player> m_players;
    private List<int> m_playerOrder;
    private int m_playerAmount;
	// Use this for initialization
	void Start ()
    {
        m_ready = new bool[4];
        m_players = new List<Player>();
        m_playerOrder = new List<int>();

        m_hud.gameObject.SetActive(false);

        foreach(MonoBehaviour current in m_gameplayScripts)
        {
            current.enabled = false;
        }
        m_spawner.enabled = false;
        m_hud.enabled = false;
        m_pauseMenu.enabled = false;
        m_camera.enabled = false;

        m_testButtons = new KeyCode[4];
        m_testButtons[0] = KeyCode.H;
        m_testButtons[1] = KeyCode.J;
        m_testButtons[2] = KeyCode.K;
        m_testButtons[3] = KeyCode.L;

        m_controllers = new XboxController[4];
        for (int i = 0; i < 4; i++)
        {
            m_controllers[i] = (XboxController)(i + 1);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < 4; i++)
        {
            if (XCI.GetButtonDown(XboxButton.A, m_controllers[i]) || Input.GetKeyDown(m_testButtons[i]))
            {
                if (!m_playerOrder.Contains(i))
                {
                    m_playerSlots[m_playerOrder.Count].text = "PRESS (A) TO READY";
                    m_playerOrder.Add(i);
                    m_startGame.enabled = false;
                }
                else
                {
                    m_ready[m_playerOrder.IndexOf(i)] = true;
                    m_playerSlots[m_playerOrder.IndexOf(i)].text = "READY";
                }
            }
        }

        int selected = m_playerOrder.Count;
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
            m_startGame.enabled = true;
            if (XCI.GetButtonDown(XboxButton.A, m_controllers[m_playerOrder[0]]) || Input.GetKeyDown(m_testButtons[m_playerOrder[0]]))
            {
                for (int i = 0; i < m_playerOrder.Count; i++)
                {
                    GameObject currentPlayer = Instantiate(m_playerPrefabs[i >= m_playerPrefabs.Length ? m_playerPrefabs.Length - 1 : i], m_spawnPoints[i].position, m_spawnPoints[i].rotation);
                    Player playerScript = currentPlayer.GetComponent<Player>();

                    playerScript.m_playerNumber = m_playerOrder[i] + 1;
                    playerScript.m_hud = m_hud;

                    m_players.Add(playerScript);                    
                }

                m_spawner.m_players = m_players.ToArray();
                m_hud.m_players = m_players.ToArray();
                m_pauseMenu.m_players = m_players.ToArray();

                List<Actor> cameraTargets = new List<Actor>();
                foreach (Actor current in m_camera.m_Targets)
                {
                    cameraTargets.Add(current);
                }
                foreach (Player current in m_players)
                {
                    cameraTargets.Add(current);
                }
                m_camera.m_Targets = cameraTargets.ToArray();

                foreach (MonoBehaviour current in m_gameplayScripts)
                {
                    current.enabled = true;
                }
                m_spawner.enabled = true;
                m_hud.enabled = true;
                m_pauseMenu.enabled = true;
                m_camera.enabled = true;

                m_hud.gameObject.SetActive(true);

                this.enabled = false;
                gameObject.SetActive(false);
            }
        }
    }
}
