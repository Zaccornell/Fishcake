using System.Collections;
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
    public Player[] m_players;
    public Spawner m_spawner;
    public PieKing m_king;
    public HUD m_hud;
    public ScreenShake m_screenShake;
    public CustomInputModule m_inputModule;
    public MonoBehaviour[] m_gameplayScripts;
    public AudioSource m_audioSource;
    public AudioClip m_lobbyMusic;

    private bool m_optionOpen;
    private bool m_toggledThisFrame;

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
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleObjects();
        }
        if (m_active && XCI.GetButtonDown(XboxButton.Start, m_inputModule.m_controller))
        {
            ToggleObjects();

            m_inputModule.m_controller = XboxController.All;
        }
        for (int i = 1; i <= 4; i++)
        {
            if (!m_active)
            {
                if (XCI.GetButtonDown(XboxButton.Start, (XboxController)i))
                {
                    ToggleObjects();
                    
                    m_inputModule.m_controller = (XboxController)i;                                                             
                }
            }
        }
        
        if (m_active)
        {
            if (XCI.GetButtonDown(XboxButton.B, m_inputModule.m_controller))
            {
                if (m_optionOpen)
                {
                    BackButton();
                }
                else
                {
                    ToggleObjects();
                }
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

        m_toggledThisFrame = false;
	}

    /*
     * Handles resetting all values in the level
     */
    public void ResetClick()
    {
        //foreach (Player current in m_players)
        //{
        //    current.ResetValues();
        //    current.Respawn();
        //}
        //m_spawner.ResetValues();
        //m_king.ResetValues();
        //foreach (Player current in m_players)
        //{
        //    XInputDotNetPure.GamePad.SetVibration((XInputDotNetPure.PlayerIndex)current.m_playerNumber - 1, 0, 0); //. set the vibration stregnth 
        //}
        SceneManager.LoadScene(1);
        //Time.timeScale = 1;
        ToggleObjects();
    }

    /*
     * Handles quitting back to the main menu
     */
    public void QuitClick()
    {
        ToggleObjects();
        SceneManager.LoadScene(0);
    }

    /*
     * Starts the objects back up when resuming
     */
    public void ResumeClick()
    {
        ToggleObjects();
    }

    /*
     * Toggles all objects between active and inactive
     */
    private void ToggleObjects()
    {
        if (!m_toggledThisFrame)
        {
            if (m_active)
            {
                m_hud.m_audioSource.loop = false;
                m_hud.m_audioSource.Stop();
            }
            else
            {
                if (m_lobbyMusic != null)
                {
                    m_audioSource.loop = true;
                    m_audioSource.clip = m_lobbyMusic;
                    m_audioSource.Play();
                }
            }
            foreach (Player current in m_players)
            {
                XInputDotNetPure.GamePad.SetVibration((XInputDotNetPure.PlayerIndex)current.m_playerNumber - 1, 0, 0); //. set the vibration stregnth 
            }

            foreach (GameObject child in m_children)
            {
                child.SetActive(!m_active);
            }
            foreach (GameObject opchild in m_optionMenu)
            {
                opchild.SetActive(false);
            }
            foreach (MovingActor current in m_spawner.m_enemies)
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

            if (m_active)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = 0f;
            }

            m_resumeButton.Select();
            m_resumeButton.OnSelect(null);
            m_king.enabled = m_active;
            m_spawner.enabled = m_active;
            m_hud.gameObject.SetActive(m_active);
            m_screenShake.enabled = m_active;
            Physics.autoSimulation = m_active;

            if (m_active)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }

            Cursor.visible = !m_active; // makes cursor visable
            m_active = !m_active;

            m_toggledThisFrame = true;
        }
    }

    /*
     * Handles opening up the option menu
     */
    public void OptionButton()
    {
        foreach (GameObject child in m_children)
        {
            child.SetActive(false);
        }
        foreach (GameObject opchild in m_optionMenu)
        {
            opchild.SetActive(true);
        }

        m_backButton.Select();
        m_optionOpen = true;
    }

    /*
     * Handles returning to the pause menu from the options menu
     */
    public void BackButton()
    {
        foreach (GameObject child in m_children)
        {
            child.SetActive(true);
        }
        foreach (GameObject opchild in m_optionMenu)
        {
            opchild.SetActive(false);
        }
        m_resumeButton.Select();
        m_optionOpen = false;
    }
}
