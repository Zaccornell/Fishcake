using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHaelth : MonoBehaviour {

    public Text[] displayHealth;
    public Actor[] Health;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < Health.Length; i++)
        {

            displayHealth[i].text = Health[i].Health.ToString();
        }
	}
}
