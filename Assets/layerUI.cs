using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class layerUI : MonoBehaviour {

	private static layerUI _instance;
	public static layerUI Instance{
		get
		{
			return _instance;
		}
	}

	public delegate void layerUIEvent();
	public static event layerUIEvent layerUIPressed;

	private Animator buttonState;

	void Awake(){
		//if (_instance == null) {
		_instance = this;
		//}
	}

	// Use this for initialization
	void Start () {
		buttonState = this.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(){
		// broadcast the onpressed event
		layerUIPressed ();
		// handle button animation states
		if (!buttonState.GetBool ("restToActive")) {
			buttonState.SetBool ("restToActive", true);
		} else {
			buttonState.SetBool ("restToActive", false);
		}

	}
}
