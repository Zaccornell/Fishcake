using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNumberDisplay : MonoBehaviour
{
    private Text m_text;

	// Use this for initialization
	void Start ()
    {
        m_text = GetComponent<Text>();	
	}

    // Update is called once per frame
    void Update ()
    {
        Color color = m_text.color;
        color.a -= 0.005f;
        m_text.color = color;

        if (color.a <= 0)
        {
            gameObject.SetActive(false);
        }
	}
}
