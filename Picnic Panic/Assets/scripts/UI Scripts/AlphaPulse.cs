﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaPulse : MonoBehaviour
{
    public float m_alphaMin;
    public float m_alphaMax;
    public float m_length;

    private Image m_image;
    private float m_timer;
    private bool m_countUp;
	// Use this for initialization
	void Start ()
    {
        m_image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_timer += Time.deltaTime * (m_countUp ? 1 : -1);
		if (m_timer <= 0 || m_timer >= m_length)
            m_countUp = !m_countUp;

        Color c = m_image.color;
        c.a = Mathf.Lerp(m_alphaMin, m_alphaMax, m_timer / m_length) / 255;
        m_image.color = c;
	}
}
