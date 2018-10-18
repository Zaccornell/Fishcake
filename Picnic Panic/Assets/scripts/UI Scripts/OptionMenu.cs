using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{

    public Toggle m_invul;
    public PlayerOptions m_pOp;
    public Toggle m_vibration;


    // Use this for initialization
    void Start ()
    {
        m_pOp = PlayerOptions.Instance;
        

	}
	
	// Update is called once per frame
	void Update ()
    {

        m_pOp.m_invulToggle = m_invul.isOn;
        m_pOp.m_vibrationToggle = m_vibration.isOn;

	}

    // gose back to main menu scene
    public void BackClick()
    {
        SceneManager.LoadScene(0); // open Main menu Scene
    }


}
