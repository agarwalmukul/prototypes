using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class AnimationControl : MonoBehaviour {

	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		anim.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0.9f) {
			Console.WriteLine ("test");
			anim.enabled = true;
		} else {
			anim.enabled = false;
		}
	}
}
