using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

/*
 * Author: John Plant / Bradyn Corkill
 * Date: 2018/11/29
 */
public class OptionMenu : MonoBehaviour
{

    public Toggle m_invul;
    public PlayerOptions m_pOp;
    public Toggle m_vibration;
    public Toggle m_friendlyFire;
    public Toggle m_screenShake;
    public Slider m_master;
    public Slider m_SFX;
    public Slider m_music;
    public AudioMixer m_audioMixer;
    public Toggle m_cutScene;
    public Button m_creditButton;
    public GameObject[] m_credit;
    public Button m_backToOption;
    public GameObject[] m_OptionMenu;


    // Use this for initialization
    void Start ()
    {
        m_pOp = PlayerOptions.Instance;  
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_pOp.m_screenShake = m_screenShake.isOn;
        m_pOp.m_firendlyFire = m_friendlyFire.isOn;
        m_pOp.m_invulToggle = m_invul.isOn;
        m_pOp.m_vibrationToggle = m_vibration.isOn;
        if (m_cutScene != null)
        {
            m_pOp.m_cutsceneToggle = m_cutScene.isOn;
        }
        

	}

    // gose back to main menu scene
    public void BackClick()
    {
        SceneManager.LoadScene(0); // open Main menu Scene
    }


    public void MusicVolumeChanged()
    {
        m_audioMixer.SetFloat("Music Volume", m_music.value);
    }
    public void SFXVolumeChanged()
    {
        m_audioMixer.SetFloat("SFX Volume", m_SFX.value);
    }
    public void MasterVolumeChanged()
    {
        m_audioMixer.SetFloat("Master Volume", m_master.value);
    }

    public void CreditButton()
    {
        foreach (GameObject child in m_OptionMenu)
        {
            child.SetActive(false);
        }
        foreach (GameObject current in m_credit)
        {
            current.SetActive(true);
        }
    }

    public void BackToOption()
    {
        foreach (GameObject child in m_OptionMenu)
        {
            child.SetActive(true);
        }
        foreach (GameObject current in m_credit)
        {
            current.SetActive(false);
        }
    }
}
