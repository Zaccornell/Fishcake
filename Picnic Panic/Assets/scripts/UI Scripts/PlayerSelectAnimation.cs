using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectAnimation : MonoBehaviour
{
    public GameObject[] m_states;
    public float m_length;
    public float m_edgeBuffer;

    private bool m_countUp;
    private float m_timer;
    private RectTransform m_rectTransform;

    // Use this for initialization
	void Start ()
    {
        m_rectTransform = gameObject.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_countUp)
        {
            m_timer += Time.deltaTime;
        }
        if (!m_countUp)
        {
            m_timer -= Time.deltaTime;
        }
        if (m_timer <= 0)
        {
            m_countUp = true;
            m_states[0].SetActive(true);
            m_states[1].SetActive(false);
        }
        if (m_timer >= m_length)
        {
            m_countUp = false;
            m_states[0].SetActive(false);
            m_states[1].SetActive(true);
        }
        m_rectTransform.position = Vector2.Lerp(new Vector2(-m_edgeBuffer + m_rectTransform.rect.width, m_rectTransform.position.y), new Vector2(Screen.width + m_edgeBuffer - m_rectTransform.rect.width, m_rectTransform.position.y), m_timer / m_length);   
	}
}
