using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSelect : MonoBehaviour
{
    public Image[] m_children;
    public GameObject m_playerPrefab;
    public Spawner m_spawner;
    public HUD m_hud;
    public PauseMenu m_pauseMenu;
    public CameraControl m_camera;
    public MonoBehaviour[] m_gameplayScripts;

    private bool[] m_ready;
    private List<Player> m_players;
	// Use this for initialization
	void Start ()
    {
        m_ready = new bool[4];
        m_players = new List<Player>();

        m_hud.gameObject.SetActive(false);

        foreach(MonoBehaviour current in m_gameplayScripts)
        {
            current.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetButtonDown("Submit" + (i + 1).ToString()))
            {
                if (!m_children[i].enabled)
                {
                    m_children[i].enabled = true;
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
            if (m_children[i].enabled)
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
            for (int i = 0; i < 4; i++)
            {
                if (m_children[i].enabled)
                {
                    GameObject currentPlayer = Instantiate(m_playerPrefab);
                    Player playerScript = currentPlayer.GetComponent<Player>();

                    playerScript.m_playerNumber = i;
                    playerScript.m_hud = m_hud;
                    playerScript.Respawn();

                    m_players.Add(playerScript);                    
                }
            }
            m_spawner.m_players = m_players.ToArray();
            m_hud.m_players = m_players.ToArray();
            m_pauseMenu.m_players = m_players.ToArray();

            List<Transform> cameraTargets = new List<Transform>();
            foreach(Transform current in m_camera.m_Targets)
            {
                cameraTargets.Add(current);
            }
            foreach(Player current in m_players)
            {
                cameraTargets.Add(current.transform);
            }
            m_camera.m_Targets = cameraTargets.ToArray();

            foreach (MonoBehaviour current in m_gameplayScripts)
            {
                current.enabled = true;
            }

            m_hud.gameObject.SetActive(true);

            this.enabled = false;
            gameObject.SetActive(false);
        }

    }
}
