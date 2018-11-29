using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public Scrollbar m_scrollbar;
    public float m_offset;

    private Vector3 m_originalPosition;
	// Use this for initialization
	void Start ()
    {
        m_originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 newPos = m_originalPosition;
        newPos.y += m_offset * m_scrollbar.value;
        transform.position = newPos;
	}
}
