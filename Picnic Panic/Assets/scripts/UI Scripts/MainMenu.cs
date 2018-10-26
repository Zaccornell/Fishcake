using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Aurthor: Bradyn Corkill 
 * Date: 2018/10/26
 */

public class MainMenu : MonoBehaviour
{

    public GameObject[] m_mainMenu;
    public GameObject[] m_OptionMenu;

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
        SceneManager.LoadScene(1);
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
    }
}
