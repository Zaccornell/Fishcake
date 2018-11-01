using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
 {

    public GameObject[] m_objects;
    public Spawner m_spawner;
    
    

 

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnEnable()
    {
        foreach (Player player in m_spawner.m_players)
        {
            player.enabled = false;
        }
        foreach (MovingActor current in m_spawner.m_enemies)
        {
            current.enabled = false;
        }
        foreach (GameObject child in m_objects) // going though and selecting every child inside of the array
        {
            child.SetActive(false); // turning the object off inside the array
        }
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(1); // loading the scene up of the gmae
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0); // loading the screen up for mainmenu
    }
}
