using System.Collections;
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
    public GameObject[] m_playerModels;
    public GameObject[] m_playerWeapons;
    public Material[] m_candyCornMaterials;
    public Material[] m_blueberryMaterials;
    public Transform[] m_spawnPoints;
    public GameObject[] m_playerNumberCovers;
    public Sprite[] m_playerNumberSprites;
    public Spawner m_spawner;
    public HUD m_hud;
    public PauseMenu m_pauseMenu;
    public EndGame m_endGame;
    public CameraControl m_camera;
    public MonoBehaviour[] m_gameplayScripts;
    public Image[] m_playerJoin;
    public AudioClip m_lobbyMusic;
    public AudioClip m_roundStart;
    public AudioSource m_audioSourceMusic;
    public AudioSource m_audioSourceSFX;
    public Transform[] m_displayLocations;
    public GameObject[] m_TEMP;
    public GameObject m_cutSence;

    private XboxController[] m_controllers;
    private KeyCode[] m_testButtons;
    private List<Player> m_players;
    private int[] m_selectedModels;
    private int[] m_selectedWeapons;
    private bool[] m_playersReady;

    public int[] SelectedWeapons
    {
        get { return m_selectedWeapons; }
    }

	// Use this for initialization
	void Start ()
    {
        m_players = new List<Player>();
        //m_playerOrder = new List<int>();
        m_playersReady = new bool[4];
        m_selectedModels = new int[4];
        m_selectedWeapons = new int[4];
        m_TEMP = new GameObject[4];

        m_hud.gameObject.SetActive(false);

        foreach(MonoBehaviour current in m_gameplayScripts)
        {
            current.enabled = false;
        }
        m_spawner.enabled = false;
        m_hud.enabled = false;
        m_pauseMenu.enabled = false;
        m_camera.enabled = false;
        if (m_lobbyMusic != null)
        {
            m_audioSourceMusic.clip = m_lobbyMusic;
            m_audioSourceMusic.loop = true;
            m_audioSourceMusic.Play();         
        }

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
        if (m_cutSence.activeSelf != true)
        {
            bool canStart = true;

            for (int i = 0; i < 4; i++)
            {
                // when a controller presses the join button
                if (XCI.GetButtonDown(XboxButton.A, m_controllers[i]) || Input.GetKeyDown(m_testButtons[i]))
                {
                    if (!m_playersReady[i])
                    {
                        m_playerSlots[i].gameObject.SetActive(false);
                        m_playerNumberCovers[i].SetActive(false);
                        m_playerJoin[i].enabled = false;
                        m_playersReady[i] = true;
                        m_startGame.enabled = false;

                        canStart = false;

                        m_TEMP[i] = Instantiate(m_playerModels[0/*Mathf.Min(m_playerModels.Length - 1, i)*/], m_displayLocations[i]);
                        Instantiate(m_playerWeapons[m_selectedWeapons[0]], m_TEMP[i].transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0));
                        m_selectedModels[i] = 0;
                        m_selectedWeapons[i] = 0;

                        m_TEMP[i].GetComponentInChildren<SkinnedMeshRenderer>().material = m_candyCornMaterials[i];

                    }
                }
            }

            int playersReady = 0;
            for (int i = 0; i < m_playersReady.Length; i++)
            {
                if (m_playersReady[i])
                {
                    playersReady++;
                }
            }

            // if the players have joined and have readied up
            if (playersReady != 0 && canStart)
            {
                m_startGame.enabled = true;

                bool start = false;
                for (int i = 0; i < m_playersReady.Length; i++)
                {
                    if (m_playersReady[i] && (XCI.GetButtonDown(XboxButton.Start, m_controllers[i]) || Input.GetKeyDown(m_testButtons[i])))
                    {
                        start = true;
                    }

                    // Scroll selected weapon up
                    if (m_playersReady[i] && (XCI.GetButtonDown(XboxButton.DPadUp, m_controllers[i]) || Input.GetKeyDown(KeyCode.UpArrow)))
                    {
                        // wrap the selected weapon back to 0 when is reaches the end of the array
                        if (m_selectedWeapons[i]++ >= m_playerWeapons.Length - 1)
                        {
                            m_selectedWeapons[i] = 0;
                        }

                        // Destroy the old weapon
                        // Character-Character_Root-Hips-Spine-Right_Arm-Right_Elbow-Right_Hand-Spatula
                        if (m_TEMP[i].transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0) != null)
                        {
                            Destroy(m_TEMP[i].transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject);
                        }
                        // Create the new weapon
                        Instantiate(m_playerWeapons[m_selectedWeapons[i]], m_TEMP[i].transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0));

                    }
                    // Scroll selected weapon down
                    if (m_playersReady[i] && (XCI.GetButtonDown(XboxButton.DPadDown, m_controllers[i]) || Input.GetKeyDown(KeyCode.DownArrow)))
                    {
                        // wrap the selected weapon back to max when it goes less than 0
                        if (m_selectedWeapons[i]-- <= 0)
                        {
                            m_selectedWeapons[i] = m_playerWeapons.Length - 1;
                        }

                        // Destroy the old weapon
                        // Character-Character_Root-Hips-Spine-Right_Arm-Right_Elbow-Right_Hand-Spatula
                        if (m_TEMP[i].transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0) != null)
                        {
                            Destroy(m_TEMP[i].transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject);
                        }
                        // Create the new weapon
                        Instantiate(m_playerWeapons[m_selectedWeapons[i]], m_TEMP[i].transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0));
                    }
                    // Scroll selected charactor right
                    if (m_playersReady[i] && (XCI.GetButtonDown(XboxButton.DPadRight, m_controllers[i]) || Input.GetKeyDown(KeyCode.RightArrow)))
                    {
                        // Wrap the selected model back to 0 when it reaches the end of the array
                        if (m_selectedModels[i]++ >= m_playerPrefabs.Length - 1)
                        {
                            m_selectedModels[i] = 0;
                        }

                        // Create the new model
                        GameObject temp = Instantiate(m_playerModels[m_selectedModels[i]], m_displayLocations[i]);

                        // Change the selected material according to the player number
                        if (m_selectedModels[i] == 0)
                        {
                            temp.GetComponentInChildren<SkinnedMeshRenderer>().material = m_candyCornMaterials[i];
                        }
                        else if (m_selectedModels[i] == 1)
                        {
                            temp.GetComponentInChildren<SkinnedMeshRenderer>().material = m_blueberryMaterials[i];
                        }

                        // Create the selected weapon
                        Instantiate(m_playerWeapons[m_selectedWeapons[i]], temp.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0));
                        
                        // Destroy the old character and add the new one to the array
                        Destroy(m_TEMP[i]);
                        m_TEMP[i] = temp;
                    }
                    // Scroll selected charactor left
                    if (m_playersReady[i] && (XCI.GetButtonDown(XboxButton.DPadLeft, m_controllers[i]) || Input.GetKeyDown(KeyCode.LeftArrow)))
                    {
                        // Wrap the selected model back to max when it goes less than 0
                        if (m_selectedModels[i]-- <= 0)
                        {
                            m_selectedModels[i] = m_playerPrefabs.Length - 1;
                        }

                        // create the new model
                        GameObject temp = Instantiate(m_playerModels[m_selectedModels[i]], m_displayLocations[i]);

                        // change the selected material according to the player number
                        if (m_selectedModels[i] == 0)
                        {
                            temp.GetComponentInChildren<SkinnedMeshRenderer>().material = m_candyCornMaterials[i];
                        }
                        else if (m_selectedModels[i] == 1)
                        {
                            temp.GetComponentInChildren<SkinnedMeshRenderer>().material = m_blueberryMaterials[i];
                        }

                        // create the selected weapon
                        Instantiate(m_playerWeapons[m_selectedWeapons[i]], temp.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0));

                        // Destroy the old character and add the new one to the array
                        Destroy(m_TEMP[i]);
                        m_TEMP[i] = temp;
                    }
                }

                // When player one presses the start button
                if (start)
                {
                    // instantiate each player
                    for (int i = 0; i < m_playersReady.Length; i++)
                    {
                        if (m_playersReady[i])
                        {
                            // create the player from the selected model
                            GameObject currentPlayer = Instantiate(m_playerPrefabs[m_selectedModels[i]], m_spawnPoints[i].position, m_spawnPoints[i].rotation);
                            // create the weapon from the selected weapon
                            GameObject weapon = Instantiate(m_playerWeapons[m_selectedWeapons[i]], currentPlayer.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0));

                            // Change the material according to the player number
                            SkinnedMeshRenderer renderer = currentPlayer.GetComponentInChildren<SkinnedMeshRenderer>();
                            if (m_selectedModels[i] == 0)
                            {
                                renderer.material = m_candyCornMaterials[i];
                            }
                            else if (m_selectedModels[i] == 1)
                            {
                                renderer.material = m_blueberryMaterials[i];
                            }

                            // setup some variables in the player script
                            Player playerScript = currentPlayer.GetComponent<Player>();
                            playerScript.m_playerNumber = i + 1;
                            playerScript.m_hud = m_hud;
                            playerScript.m_audioSourceSFX = m_audioSourceSFX;
                            playerScript.m_weaponCollider = weapon.GetComponent<Collider>();

                            if (m_playerNumberSprites.Length > 0)
                            {
                                playerScript.DisplayPlayerNumber(m_playerNumberSprites[Mathf.Min(i, m_playerNumberSprites.Length - 1)]);
                            }

                            m_players.Add(playerScript);
                        }
                    }

                    // give objects access to the players
                    m_spawner.m_players = m_players.ToArray();
                    m_hud.m_players = m_players.ToArray();
                    m_pauseMenu.m_players = m_players.ToArray();
                    m_endGame.m_players = m_players.ToArray();

                    // Add the players to the camera's targets
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

                    // Enable the other scripts
                    m_spawner.enabled = true;
                    m_hud.enabled = true;
                    m_pauseMenu.enabled = true;
                    m_camera.enabled = true;

                    m_hud.gameObject.SetActive(true);

                    // Play the ingame music
                    m_audioSourceMusic.loop = false;
                    m_audioSourceMusic.Stop();
                    if (m_roundStart != null)
                    {
                        m_audioSourceMusic.PlayOneShot(m_roundStart);
                    }
                    this.enabled = false;
                    gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (XCI.GetButtonDown(XboxButton.Start) || Input.GetKeyDown(KeyCode.H) || PlayerOptions.Instance.m_cutsceneToggle)
            {
                m_cutSence.SetActive(false);
            }
        }

    }

}
