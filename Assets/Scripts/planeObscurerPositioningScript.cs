using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeObscurerPositioningScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.eulerAngles = new Vector3 (this.transform.eulerAngles.x, -5.365f, 0.0f);
	}

	// 0.166, 0.05371, -0.044
	// 0, -5.365. 0


	// 0.166, 0.3094569, 0.3752506
	// - 60, - 5.365, 0
}
