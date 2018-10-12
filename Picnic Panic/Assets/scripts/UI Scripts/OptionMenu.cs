using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{

    public Toggle m_invul;
    public Player m_player;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // gose back to main menu scene
    public void BackClick()
    {
        SceneManager.LoadScene(0); // open Main menu Scene
    }

    // turning Invul off and on
    public void InvulToggle()
    {
       
       

    }

}
