using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Author: John Plant
 * Date: 2018/11/29
 */
public class PlayerNumberDisplay : MonoBehaviour
{
    private Image m_image;

	// Use this for initialization
	void Start ()
    {
        m_image = GetComponent<Image>();	
	}

    // Update is called once per frame
    void Update ()
    {
        // Gradually lowers the alpha of the image
        Color color = m_image.color;
        color.a -= 0.005f;
        m_image.color = color;

        if (color.a <= 0)
        {
            gameObject.SetActive(false);
        }
	}
}
