using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetonhit : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    	
	}

    private void OnCollisionEnter(Collision col)
    {
        if (col.rigidbody.tag == "Player")
        {
            PersonalityController.thisPersonalityController.ResetGame(); //reset level when player hits the ground
        }
    }
}
