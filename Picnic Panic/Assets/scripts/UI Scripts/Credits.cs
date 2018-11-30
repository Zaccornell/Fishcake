using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public Scrollbar m_scrollbar;
    public float m_offset;

    private float m_originalOffset;
    private RawImage m_image;
	// Use this for initialization
	void Start ()
    {
        m_image = GetComponent<RawImage>();
        m_originalOffset = m_image.uvRect.y;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float newOffset = m_originalOffset;
        newOffset += m_offset * m_scrollbar.value;

        Rect newRect = m_image.uvRect;
        newRect.y = newOffset;
        m_image.uvRect = newRect;
	}
}
