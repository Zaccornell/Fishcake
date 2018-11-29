using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public Text m_warning;
    public float m_length;



    private float m_timer;


	// Use this for initialization
	void Start ()
    {
        m_timer = m_length;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_timer -= Time.deltaTime;
        
        Color c = m_warning.color;

        c.a = m_timer / m_length;

        m_warning.color = c;
    }
}
