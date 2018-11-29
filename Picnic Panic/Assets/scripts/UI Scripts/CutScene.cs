using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Author: Bradyn Corkill
 * Date: 2018/11/29
 */
 
public class CutScene : MonoBehaviour
{
    public Sprite[] m_sprite;
    public float m_length;

    private Image m_cutScene;
    private int m_index;
    private float m_timer;



	// Use this for initialization
	void Start ()
    {
        m_cutScene = GetComponent<Image>(); // getting the image component
        m_timer = m_length; // setting the timer to length 
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_timer -= Time.deltaTime; // setting timer to count down in delta time 
        if (m_timer <= 0)
        {
            m_index++; // incressing the index
            if (m_index >= m_sprite.Length)
            {
                m_index = 0; // setting the index as one 
            }
            m_cutScene.sprite = m_sprite[m_index]; // setting the image to a sprite 
            m_timer = m_length; // resetting the timer to length 
        }
	}
}
