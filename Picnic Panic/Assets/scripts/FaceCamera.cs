using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Camera m_camera;

	// Use this for initialization
	void Start ()
    {
        m_camera = GameObject.Find("Camera_MainCamera_001").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.forward = m_camera.transform.forward;
	}
}
