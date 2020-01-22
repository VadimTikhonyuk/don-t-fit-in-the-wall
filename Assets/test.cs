using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour 
{
	public Transform target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ((transform.position.y + 0.4f) > target.position.y && transform.position.y - 0.4f  < target.position.y) {	
			Debug.Log ("1");
		}
	}
}
