using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{

    public Toggle m_invul;
    public Player m_player;
    public PlayerOptions m_pOp;

    // Use this for initialization
    void Start ()
    {
        m_pOp = PlayerOptions.Instance;
	}
	
	// Update is called once per frame
	void Update ()
    {

        m_pOp.m_invulToggle = m_invul.isOn;

	}

    // gose back to main menu scene
    public void BackClick()
    {
        SceneManager.LoadScene(0); // open Main menu Scene
    }


}
