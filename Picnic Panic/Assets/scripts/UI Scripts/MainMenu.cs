using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

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
        SceneManager.LoadScene(2); // open Option Scene
    }
    
    public void QuitClick()
    {
        Application.Quit();
    }
}
