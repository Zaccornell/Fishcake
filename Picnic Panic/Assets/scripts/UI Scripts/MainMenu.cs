﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Aurthor: Bradyn Corkill 
 * Date: 2018/10/26
 */

public class MainMenu : MonoBehaviour
{

    public GameObject[] m_mainMenu;
    public GameObject[] m_OptionMenu;
    public Button m_backButton;
    public Button m_playButton;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void PlayClick()
    {         
        SceneManager.LoadScene(Random.Range(1, SceneManager.sceneCountInBuildSettings - ));
    }
    // open Option once the button is clicked
    public void OptionClick()
    {
        foreach (GameObject current in m_mainMenu)
        {
            current.SetActive(false);
        }
        foreach (GameObject child in m_OptionMenu)
        {
            child.SetActive(true);
        }
        m_backButton.Select();
    }
    
    public void QuitClick()
    {
        Application.Quit();
    }
    
    public void BackToMainMenu()
    {
        foreach (GameObject current in m_mainMenu)
        {
            current.SetActive(true);
        }
        foreach (GameObject child in m_OptionMenu)
        {
            child.SetActive(false);
        }
        m_playButton.Select();
    }
}
