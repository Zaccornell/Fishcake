using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: John Plant
 * Date: 2018/11/29
 */
public class CameraMovement : MonoBehaviour
{
    public GameObject m_target = null;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = new Vector3(m_target.transform.position.x, 16, m_target.transform.position.z - 17.5f);
    }
}
